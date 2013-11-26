Option Explicit On
Option Strict On

'Imports System.Drawing
'Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
'Imports System.Drawing.Bitmap
'Imports System.Windows.Media.Imaging
Imports System.Configuration
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Reflection
Imports SFTP_Test
Imports System.ComponentModel
Imports Microsoft.VisualBasic


Public Class CRMPhotoImportHelper

	<System.Runtime.InteropServices.DllImport("gdi32.dll")> _
	Public Shared Function DeleteObject(ByVal hObject As IntPtr) As Boolean
	End Function

	Private _ProcessStatus As ProcessStatusType = ProcessStatusType.Stopped

	'  11/18/13 Memphis:  changes to implement TK-01168
	'- ?? way to identify if this is a new photo or an older one:
	'  •User will have to check an "Archive" checkbox if the photos are 'old'
	'  •Update the CSV file definition to add the new fields & calculate the photo type based on "archive" bit
	'  •Importer Title = 2013 Learning Letter
	'  •Archive = not checked
	'  •Change the attachment type to "Child PHoto - Full Body" and "Child Photo - Headshot".
	'  •Add new attachment of "current" headshot and fullbody
	'  •Add the title from the importer = 2013 Learning Letter
	'  •Update the Photo year field = 2013
	' - New Fields in the CSV file:
	'   - Archive:   True or False
	'   - Unusable: True or False 
	'   - PhotoYear:  integer

	Private Property ProcessStatus() As ProcessStatusType
		Get
			Return _ProcessStatus
		End Get
		Set(ByVal value As ProcessStatusType)
			_ProcessStatus = value
		End Set
	End Property

	Public Enum ProcessStatusType
		Stopped
		Running
		Paused
	End Enum

#Region "PrivateVariables"
	Private _TargetWidth As Integer = 0
	Private _TargetHeight As Integer = 0
	Private _PaddingTop As Decimal = 0
	Private _PaddingSide As Decimal = 0

	' this is the class that handles all the SFTP stuff
	Private _sftpClient As New OcmSFTPClient()

	' background worker for GUI:
	Private _Worker As New BackgroundWorker()

	' holds the SFTP specific items:
	' ************************************************************************************************************
	' These will need to be moved to the App.Config file before rollout!!!
	Private _host As String = String.Empty '"files.blackbaudhosting.com"
	Private _username As String = String.Empty '"MarkSe21195D@BBEC"
	Private _password As String = "P@ssword2"
	Private _workingdirectory As String = String.Empty ' "/staging/21195D/ChildPhotos"
	Private _uploadfile As String = String.Empty ' "C:\OCM\PhotoImports\2013 Photos_Processing_6_25_2013\_Import\C210608.jpg"
	Private _ftpPhotoFolderName As String = String.Empty
	Private _numberOfFiles As Integer = 0
	Private _ftpImportFileLocation As String = String.Empty
	Private _ftpCRMPhotoFileLocation As String = String.Empty

	Private _pictureType As String = String.Empty
	Private _defaultSourceFolder As String = String.Empty

	Private _ftpFolderAlreadyFormatted As Boolean


	' holds the image files:
	Private _imageFiles As String() = {}

	'holds the photo attachment type values:  gets used based on the Archive checkbox
	Private Const _PHOTOATTACHMENTTYPE_BODYCURRENT As String = "Child Photo - Full Body - Current"
	Private Const _PHOTOATTACHMENTTYPE_HEADSHOTCURRENT As String = "Child Photo - Headshot - Current"
	Private Const _PHOTOATTACHMENTTYPE_BODY As String = "Child Photo - Full Body"
	Private Const _PHOTOATTACHMENTTYPE_HEADSHOT As String = "Child Photo - Headshot"
	Private Const _PHOTOATTACHMENTTYPE_BODY_UNUSABLE As String = "Child Photo - Full Body - Unusable"
	Private Const _PHOTOATTACHMENTTYPE_HEADSHOT_UNUSABLE As String = "Child Photo - Headshot - Unusable"

	'these will be populated by one of the above values, based on the photo type and archive bit
	Private _headshotPhotoAttachmentType As String = String.Empty
	Private _fullBodyPhotoAttachmentType As String = String.Empty

#End Region


	Private Sub CRMPhotoImportHelper_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		e.Cancel = False
		If Me._ProcessStatus <> ProcessStatusType.Stopped Then
			If MsgBox("Headshots are currently being processed.  Are you sure you want to Exit?", MsgBoxStyle.YesNo, Me.Text) <> MsgBoxResult.Yes Then
				e.Cancel = True
			Else
				Me._ProcessStatus = ProcessStatusType.Stopped
			End If
		End If
	End Sub


	Private Sub CRMPhotoImportHelper_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
		_ftpFolderAlreadyFormatted = False

		Me.lblChildID.Text = ""
		Me.lblStatus.Text = ""

		Me.txtMaxPhotos.Text = "5000"

		SetButtons()

		' Memphis 7/19/13 for the new SFTP functionality
		progressBar1.Maximum = 100
		progressBar1.Style = ProgressBarStyle.Blocks
		progressBar1.[Step] = 1
		progressBar1.Value = 0
		' user can't transfer until they select everything else & process the folder:
		btnTransfer.Enabled = False
		btnSelectFolder.Enabled = False

		GetConfigEntries()

		Me.txtBlackbaudPhotoLocation.Text = _ftpCRMPhotoFileLocation '"\\bbhcifs01\clientdata\Production\21195P\ChildPhotos" '"\\bbhcifs01\clientdata\staging\21195D\ChildPhotos" 
		Me.txtImportFileFolderName.Text = _ftpImportFileLocation
		Me.txtSourceFolderName.Text = _defaultSourceFolder

		AddHandler _sftpClient.FileUploadCompleted, AddressOf fileUploadCompleted
		AddHandler _sftpClient.FileUploading, AddressOf fileUploading

	End Sub


	Private Sub OpenFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
		Dim dlg As FolderBrowserDialog

		dlg = New FolderBrowserDialog()
		dlg.SelectedPath = txtSourceFolderName.Text
		If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			txtSourceFolderName.Text = dlg.SelectedPath
		End If
	End Sub

	Private Sub cmdProcessFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdProcessFolder.Click

		Dim parmsValid As Boolean = True

		Dim pictureTitle As String = String.Empty
		Dim maxFilesToProcess As Integer = 0
		_ftpCRMPhotoFileLocation = _ftpCRMPhotoFileLocation + "\" + FormatFTPFolderName()
		Me.txtBlackbaudPhotoLocation.Text = _ftpCRMPhotoFileLocation
		Dim fileLocation As String = Me.txtBlackbaudPhotoLocation.Text

		' Check a for child id (with and without the -H)
		Dim validFileNameFullbodyRegEx As New Regex("^[cC][0-9][0-9][0-9][0-9][0-9][0-9]$")
		Dim validFileNameHeadshotRegEx As New Regex("^[cC][0-9][0-9][0-9][0-9][0-9][0-9]-H$")

		Dim ProcessedSubFolder As String = "_Import"
		Dim ExceptionSubFolder As String = "_Exceptions"

		Dim NumberOfHeadshots As Integer = 0
		Dim NumberOfFullBody As Integer = 0

		Dim NumberOfFilesProcessed As Integer = 0
		Dim NumberOfInvalidFiles As Integer = 0

		Dim newFileName As String = String.Empty
		Dim ProcessedFiles As String = String.Empty

		' for new fields 11/18/13 Memphis:
		Dim photoUnusable As Boolean = Me.unusableCheckBox.Checked
		Dim archivePhoto As Boolean = Me.archiveCheckBox.Checked
		Dim photoYear As Integer = Convert.ToInt32(Strings.Left(Me.cbPictureTitle.Text, 4))

		Dim headshotRow As String = String.Empty
		Dim fullBodyRow As String = String.Empty

		System.Diagnostics.Debug.WriteLine(photoUnusable)
		System.Diagnostics.Debug.WriteLine(archivePhoto)

		If cbPictureTitle.Text = String.Empty Then
			MsgBox("Missing Picture Title", MsgBoxStyle.Critical, "Please enter a Picture Title")
			parmsValid = False
		Else
			pictureTitle = cbPictureTitle.Text	' 2012 Child Profile"	'"""BLACKBAUDHOST\Import21195P,21195P@imp123PWd"""	 '"2012 Health Update"
		End If

		If IsNumeric(Me.txtMaxPhotos.Text) Then
			maxFilesToProcess = CInt(Me.txtMaxPhotos.Text)
		Else
			MsgBox("Please enter a number for the Maximum number of photos to process in this batch.", MsgBoxStyle.Critical, Me.Text & " - Processing Error")
			parmsValid = False
		End If

		If Not Directory.Exists(Me.txtSourceFolderName.Text) Then
			MsgBox("The Source Folder does not exist.  Please select a valid folder.", MsgBoxStyle.Critical, Me.Text & " - Processing Error")
			parmsValid = False
		End If


		If parmsValid Then
			Dim PathName As String = txtSourceFolderName.Text

			If Not PathName.EndsWith("\") Then
				PathName &= "\"
			End If

			Dim importFilePathName As String = txtImportFileFolderName.Text

			If Not importFilePathName.EndsWith("\") Then
				importFilePathName &= "\"
			End If

			Dim ExceptionPath As String = PathName & ExceptionSubFolder
			Dim ProcessedPath As String = PathName & ProcessedSubFolder

			If Not Directory.Exists(ProcessedPath) Then
				Directory.CreateDirectory(ProcessedPath)
			End If

			If Not Directory.Exists(ExceptionPath) Then
				Directory.CreateDirectory(ExceptionPath)
			End If


			Dim importFile As StreamWriter = New StreamWriter(importFilePathName & "ChildPhotoImport-" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".csv")
			importFile.WriteLine("ChildLookupID,AttachmentType,PictureTitle,FileName,FileLocation,Archive,Unusable,PhotoYear")

			Dim exceptionFile As StreamWriter = New StreamWriter(importFilePathName & "ChildPhotoImport-Exception-" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".csv")
			exceptionFile.WriteLine("FileName,Exception")

			cmdProcessFolder.Enabled = False

			Me.ProcessStatus = ProcessStatusType.Running

			SetButtons()

			lblStatus.Text = "Adding to import file..."

			txtSourceFolderName.Focus()
			txtSourceFolderName.SelectionLength = 0
			rtbMessage.Text = ""

			For Each ImageFileWithPath As String In IO.Directory.GetFiles(PathName)
				Application.DoEvents()

				Do While Me.ProcessStatus <> ProcessStatusType.Running
					Application.DoEvents()
					If Me.ProcessStatus = ProcessStatusType.Stopped Then
						SetButtons()
						lblStatus.Text = "Import file creation Cancelled."
						Exit Sub
					End If
				Loop


				If Path.GetExtension(ImageFileWithPath).ToLower <> ".jpg" Then
					' skip thumbs.db file
					If Path.GetFileName(ImageFileWithPath).ToLower <> "thumbs.db" Then
						Rename(ImageFileWithPath, ExceptionPath & "\" & Path.GetFileName(ImageFileWithPath))
						NumberOfInvalidFiles += 1
						exceptionFile.WriteLine(Path.GetFileName(ImageFileWithPath) & "," & "Not a jpg file")
						rtbMessage.AppendText(Path.GetFileName(ImageFileWithPath) & vbTab & "Not a jpg file" & vbNewLine)
					End If
				Else
					Application.DoEvents()

					NumberOfFilesProcessed += 1

					lblNumProcessedValue.Text = NumberOfFilesProcessed.ToString


					If validFileNameFullbodyRegEx.IsMatch(Path.GetFileNameWithoutExtension(ImageFileWithPath)) Then
						NumberOfFullBody += 1
						newFileName = Path.GetFileNameWithoutExtension(ImageFileWithPath).ToUpper & Path.GetExtension(ImageFileWithPath)
						Try
							'set the correct photo type based on archive checkbox value:
							If archivePhoto = True Then
								_fullBodyPhotoAttachmentType = _PHOTOATTACHMENTTYPE_BODY
							Else
								_fullBodyPhotoAttachmentType = _PHOTOATTACHMENTTYPE_BODYCURRENT
							End If

							If photoUnusable = True Then
								_fullBodyPhotoAttachmentType = _PHOTOATTACHMENTTYPE_BODY_UNUSABLE
							End If

							Rename(ImageFileWithPath, ProcessedPath & "\" & newFileName)
							fullBodyRow = String.Format("{0},{1},{2},{3},{4},{5},{6},{7}", Path.GetFileNameWithoutExtension(ImageFileWithPath).ToUpper, _fullBodyPhotoAttachmentType, pictureTitle, newFileName, fileLocation, Me.archiveCheckBox.Checked, Me.unusableCheckBox.Checked, photoYear)

							importFile.WriteLine(fullBodyRow)
							'importFile.WriteLine(Path.GetFileNameWithoutExtension(ImageFileWithPath).ToUpper & "," & _fullBodyPhotoAttachmentType & "," & pictureTitle & "," & newFileName & "," & fileLocation & ", " & archivePhoto & "," & If(photoUnusable, "True", "False") & "," & photoYear)	'ProcessedPath)

						Catch ex As Exception
							NumberOfInvalidFiles += 1

							exceptionFile.WriteLine(Path.GetFileName(ImageFileWithPath) & "," & "File move failed. " & ex.Message)
							rtbMessage.AppendText(Path.GetFileName(ImageFileWithPath) & vbTab & "File move failed. " & ex.Message & vbNewLine)
						End Try


					ElseIf validFileNameHeadshotRegEx.IsMatch(Path.GetFileNameWithoutExtension(ImageFileWithPath)) Then
						NumberOfFullBody += 1
						newFileName = Path.GetFileNameWithoutExtension(ImageFileWithPath).ToUpper & Path.GetExtension(ImageFileWithPath)

						Try
							'set the correct photo type based on archive checkbox value:
							If archivePhoto = True Then
								_headshotPhotoAttachmentType = _PHOTOATTACHMENTTYPE_HEADSHOT
							Else
								_headshotPhotoAttachmentType = _PHOTOATTACHMENTTYPE_HEADSHOTCURRENT
							End If

							If photoUnusable = True Then
								_headshotPhotoAttachmentType = _PHOTOATTACHMENTTYPE_HEADSHOT_UNUSABLE
							End If

							Rename(ImageFileWithPath, ProcessedPath & "\" & newFileName)
							importFile.WriteLine(Path.GetFileNameWithoutExtension(ImageFileWithPath).ToUpper.Substring(0, 7) & "," & _headshotPhotoAttachmentType & "," & pictureTitle & "," & newFileName & "," & fileLocation & ", " & archivePhoto & "," & photoUnusable & "," & photoYear) 'ProcessedPath)
						Catch ex As Exception
							NumberOfInvalidFiles += 1

							exceptionFile.WriteLine(Path.GetFileName(ImageFileWithPath) & "," & "File move failed. " & ex.Message)
							rtbMessage.AppendText(Path.GetFileName(ImageFileWithPath) & vbTab & "File move failed. " & ex.Message & vbNewLine)
						End Try
					Else
						Rename(ImageFileWithPath, ExceptionPath & "\" & Path.GetFileName(ImageFileWithPath))
						NumberOfInvalidFiles += 1

						exceptionFile.WriteLine(Path.GetFileName(ImageFileWithPath) & "," & "Invalid file name")
						rtbMessage.AppendText(Path.GetFileName(ImageFileWithPath) & vbTab & "Invalid file name" & vbNewLine)

					End If

					lblNumInvalid.Text = NumberOfInvalidFiles.ToString
					rtbMessage.ScrollToCaret()

					Me.Refresh()

					If NumberOfFilesProcessed = maxFilesToProcess Then
						Exit For
					End If
					Application.DoEvents()

				End If
			Next

			importFile.Close()
			exceptionFile.Close()

			Me.ProcessStatus = ProcessStatusType.Stopped

			SetButtons()

			lblStatus.Text = "Import file creation completed."

			' fill the image files array with the image files just created & indicate to user how many were selected for transfer:
			GetImageFiles()

			If _numberOfFiles > 0 Then
				cmdProcessFolder.Enabled = False
			End If

		End If
	End Sub


	Private Sub SetButtons()
		If Me.ProcessStatus = ProcessStatusType.Running Then
			cmdProcessFolder.Enabled = False
			cmdCancel.Visible = True
			cmdPause.Visible = True
		Else
			cmdProcessFolder.Enabled = True
			cmdCancel.Visible = False
			cmdPause.Visible = False
		End If
	End Sub


	'Private Function ResizeImage(ByVal imgToResize As Image, ByVal size As Size) As Image
	'	Dim sourceWidth As Integer = imgToResize.Width
	'	Dim sourceHeight As Integer = imgToResize.Height

	'	Dim nPercent As Decimal = 0
	'	Dim nPercentW As Decimal = 0
	'	Dim nPercentH As Decimal = 0

	'	nPercentW = (CType(size.Width, Decimal) / sourceWidth)
	'	nPercentH = (CType(size.Height, Decimal) / sourceHeight)

	'	If (nPercentH < nPercentW) Then
	'		nPercent = nPercentH
	'	Else
	'		nPercent = nPercentW
	'	End If

	'	Dim destWidth As Integer = CType((sourceWidth * nPercent), Integer)
	'	Dim destHeight As Integer = CType((sourceHeight * nPercent), Integer)

	'	Dim b As Bitmap = New Bitmap(destWidth, destHeight)
	'	Dim g As Graphics = Graphics.FromImage(b)
	'	g.InterpolationMode = InterpolationMode.HighQualityBicubic

	'	g.DrawImage(imgToResize, 0, 0, destWidth, destHeight)
	'	g.Dispose()

	'	Return b
	'End Function

	Private Sub cmdBrowseFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBrowseFolder.Click
		Dim dlg As FolderBrowserDialog

		dlg = New FolderBrowserDialog()
		dlg.SelectedPath = txtSourceFolderName.Text
		If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			txtSourceFolderName.Text = dlg.SelectedPath
		End If

		txtSourceFolderName.Focus()
	End Sub

	Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
		If Me.ProcessStatus = ProcessStatusType.Stopped Then
			Application.Exit()
		Else
			If MsgBox("Are you sure you want to stop creating import file?", MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.Yes Then
				Me.ProcessStatus = ProcessStatusType.Stopped
			End If
		End If
	End Sub

	Private Sub cmdPause_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPause.Click
		If Me.ProcessStatus = ProcessStatusType.Running Then
			Me.ProcessStatus = ProcessStatusType.Paused
			cmdPause.Text = "Resume"
			lblStatus.Text = "Paused..."
		Else
			Me.ProcessStatus = ProcessStatusType.Running
			cmdPause.Text = "Pause"
			lblStatus.Text = "Adding to import file...."
		End If
	End Sub

	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.


	End Sub

	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
		Dim dlg As FolderBrowserDialog

		dlg = New FolderBrowserDialog()
		dlg.SelectedPath = txtImportFileFolderName.Text
		If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			txtImportFileFolderName.Text = dlg.SelectedPath
		End If

		txtSourceFolderName.Focus()
	End Sub

#Region "EventHandlers"
	Private Sub fileUploadCompleted(ByVal sender As Object, ByVal e As FileUploadCompletedEventArgs)
		OutputThis("\r")
		OutputThis("\rFinished uploading the file!")
		OutputThis(String.Format("\rUploaded {0} {1} {2:#########}", e.UploadedFileName, e.FileNumber, e.BytesSent))
	End Sub

	Private Sub fileUploading(ByVal sender As Object, ByVal e As FileUploadingEventArgs)
		'var backgrounder = sender as BackgroundWorker;
		Dim progress As Integer = 100
		'progress = (Convert.ToInt32(e.BytesSentSoFar) / Convert.ToInt32(e.FileBytesTotal)) * 100;
		progress = CInt(Math.Truncate((CSng(e.BytesSentSoFar) / CSng(e.FileBytesTotal)) * 100))

		LogThis(String.Format("Bytes sent so far: {0}", e.BytesSentSoFar))
		LogThis(String.Format("File Bytes Total: {0}", e.FileBytesTotal))
		LogThis(String.Format("Progress %{0}", progress))
		LogThis(String.Format("File {0} of {1}", e.FileNumberOfTotal, e.TotalNumberFilesToTransfer))

		SetControlPropertyValue(lblFilesInProgress, "text", String.Format("Transferring {0} of {1} files...", e.FileNumberOfTotal, e.TotalNumberFilesToTransfer))

		BackgroundWorker1.ReportProgress(progress)
	End Sub

#End Region

	Private Sub OutputThis(ByVal outputMessage As String)
		'richTextBox1.AppendText(outputMessage)
		LogThis(outputMessage)
	End Sub

	Private Sub LogThis(ByVal logMessage As String)
		System.Diagnostics.Debug.WriteLine(logMessage)
	End Sub

#Region "BackgroundWorker Methods"
	''' <summary>
	''' Creates the BackgroundWorker with WorkerReportsProgress and 
	''' WorkerSupportsCancellation set to true. 
	''' Also creates the three Eventhandlers
	''' </summary>
	Public Sub CreateBackgroundWorker()
		'Set WorkerReportsProgress and WorkerSupportsCancellation to true
		_Worker.WorkerSupportsCancellation = True
		_Worker.WorkerReportsProgress = True

		'Generate the EventHandler
		AddHandler _Worker.RunWorkerCompleted, New RunWorkerCompletedEventHandler(AddressOf Worker_RunWorkerCompleted)
		AddHandler _Worker.ProgressChanged, New ProgressChangedEventHandler(AddressOf Worker_ProgressChanged)
		AddHandler _Worker.DoWork, New DoWorkEventHandler(AddressOf Worker_DoWork)

		_Worker.RunWorkerAsync()
	End Sub

	Private Sub Worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
		' wire up the event listeners to catch the events raised by the client:
		AddHandler _sftpClient.FileUploadCompleted, AddressOf fileUploadCompleted
		AddHandler _sftpClient.FileUploading, AddressOf fileUploading
		'AddHandler _sftpClient.ListingDirectory, AddressOf listingDirectory

		' setup the transfer properties needed by the client:
		_sftpClient.HostName = _host
		_sftpClient.UserName = _username
		_sftpClient.Password = _password
		_sftpClient.WorkingDirectory = _workingdirectory
		_sftpClient.UploadFileName = _uploadfile
		_sftpClient.PhotoFTPFolderName = _ftpPhotoFolderName  'FormatFTPFolderName()

		' perform the transfer of a single file:
		'_sftpClient.transferFile();

		' multiple files get this call:
		_sftpClient.transferFiles(_imageFiles)
	End Sub

	Private Sub Worker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
		'Create an instace of the single file progressBar
		Dim ProgressBarSingle As ProgressBar = DirectCast(Me.Controls("progressBar1"), ProgressBar)

		'Increase the value of the progressBar by e.ProgressPercentage
		ProgressBarSingle.Value = e.ProgressPercentage

		Dim ProgressBarOverall As ProgressBar = DirectCast(Me.Controls("overallProgressBar"), ProgressBar)

		'Calculate by whic the Overall ProgressBar needs to be increased,
		'so that we get a smooth rise.
		Dim PercentRise As Integer = 100 \ _numberOfFiles

		' only increment the overall bar if this single file is done
		If ProgressBarSingle.Value = 100 Then
			'Duno why it has to be here, but if it isn't the progressBarSingle
			'doesn't stop working. So leave it here with no code...
			If ProgressBarOverall.Value >= 100 Then
			Else
				'Set the SingleFile progressBar to 0, so it cann be filled again
				'ProgressBarSingle.Value = 0;

				'This is the "catch" part, so the programm doesn't throw up an error
				If ProgressBarOverall.Value + PercentRise >= 100 Then
					'Set both progress bars to 100 so it looks better.
					ProgressBarOverall.Value = 100
					ProgressBarSingle.Value = 100
				Else
					'increase the Global progressBar by the calculated amount for each file
					ProgressBarOverall.Value = ProgressBarOverall.Value + PercentRise
				End If
			End If
		End If
	End Sub

	Private Sub Worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
		Dim ProgressBarSingle As ProgressBar = DirectCast(Me.Controls("progressBar1"), ProgressBar)
		ProgressBarSingle.Value = 100

		SetControlPropertyValue(overallProgressBar, "value", 100)

		SetControlPropertyValue(btnTransfer, "enabled", True)

		SetControlPropertyValue(lblFilesInProgress, "text", "All files transferred!")

		MessageBox.Show("Image files have been transferred. You may now close this application...", "Image File Transfer", MessageBoxButtons.OK, MessageBoxIcon.Information)
	End Sub
#End Region

	''' <summary>
	''' These are used to manage the UI controls from the worker thread
	''' </summary>
	''' <param name="oControl"></param>
	''' <param name="propName"></param>
	''' <param name="propValue"></param>
	Private Delegate Sub SetControlValueCallback(ByVal oControl As Control, ByVal propName As String, ByVal propValue As Object)
	Private Sub SetControlPropertyValue(ByVal oControl As Control, ByVal propName As String, ByVal propValue As Object)
		If oControl.InvokeRequired Then
			Dim d As New SetControlValueCallback(AddressOf SetControlPropertyValue)
			oControl.Invoke(d, New Object() {oControl, propName, propValue})
		Else
			Dim t As Type = oControl.[GetType]()
			Dim props As PropertyInfo() = t.GetProperties()
			For Each p As PropertyInfo In props
				If p.Name.ToUpper() = propName.ToUpper() Then
					p.SetValue(oControl, propValue, Nothing)
				End If
			Next
		End If
	End Sub


	Private Sub GetImageFiles()
		' source should be something like this:  C:\OCM\PhotoImports\2013 Photos_Processing_6_25_2013\_Import
		_imageFiles = Directory.GetFiles(Me.txtSourceFolderName.Text + "\_Import", "*.jpg")
		_numberOfFiles = _imageFiles.Count()
		imageFilesSelectedLabel.Text = String.Format("{0} image files ready to transfer.", _numberOfFiles)
		'For Each filename As String In _imageFiles
		'	MessageBox.Show(filename)
		'Next

		If _numberOfFiles > 0 Then
			btnTransfer.Enabled = True
		Else
			MessageBox.Show("Oops! Something went wrong, there are no image files to transfer!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
		End If

	End Sub

	Private Sub btnSelectFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectFolder.Click
		OpenFileDialog1.InitialDirectory = "C:\OCM\PhotoImports"
		OpenFileDialog1.Filter = "Image Files|*.BMP;*.GIF;*.JPG;*.JPEG;*.PNG|All files (*.*)|*.*"
		OpenFileDialog1.Title = "Select Image files to Upload"
		OpenFileDialog1.CheckPathExists = True
		OpenFileDialog1.Multiselect = True
		Dim imageDialogResult As DialogResult = OpenFileDialog1.ShowDialog()

		If imageDialogResult = DialogResult.OK Then
			_imageFiles = OpenFileDialog1.FileNames
			_numberOfFiles = _imageFiles.Count()
			imageFilesSelectedLabel.Text = String.Format("{0} image files selected.", _numberOfFiles)
			'For Each imageFile As String In _imageFiles
			'	'MessageBox.Show(string.Format("You selected {0} image file!", imageFile.ToString()));
			'	OutputThis2(String.Format(vbCr & " {0}", imageFile.ToString()))
			'Next

			'turn on transfer button now if the user selected some files:
			If _numberOfFiles > 0 Then
				btnTransfer.Enabled = True
			End If
		End If

	End Sub

	Private Sub btnTransfer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTransfer.Click
		btnTransfer.Enabled = False
		lblFilesInProgress.Visible = True
		BackgroundWorker1.RunWorkerAsync(_sftpClient)
	End Sub

	Private Sub cbPictureTitle_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbPictureTitle.SelectedIndexChanged
		'set the folder name based on what picture type the user selected
		'MessageBox.Show(cbPictureTitle.Items((cbPictureTitle.SelectedIndex)).ToString())
		_pictureType = cbPictureTitle.Items((cbPictureTitle.SelectedIndex)).ToString()
		'the folder name is something like this: 2013_Child_Photos_
		_ftpPhotoFolderName = _pictureType.Replace(" ", "_") + "s_"
		'MessageBox.Show(FormatFTPFolderName())
	End Sub

	Private Function FormatFTPFolderName() As String
		If _ftpFolderAlreadyFormatted = False Then
			Dim todayDate As String = DateTime.Now.ToShortDateString()
			todayDate = todayDate.Replace("/"c, "_"c)
			_ftpPhotoFolderName = String.Format(_ftpPhotoFolderName & "{0}", todayDate).Trim()
			LogThis(String.Format(vbCr & "FTP folder for Photos will be: {0}", _ftpPhotoFolderName))
			_ftpFolderAlreadyFormatted = True
			Return _ftpPhotoFolderName
		End If
	End Function

	Private Sub GetConfigEntries()
		'sets the Photo FTP foldername using the App.config settings entries plus the type of picture title the user selected:
		' the DEVworkingdirectory = /staging/21195D/ChildPhotos
		' the PRODworkingdirectory = /Production/21195P/ChildPhotos
		' the PRODPhotoImportFileLocation = "\\bbhcifs01\clientdata\Production\21195P\ChildPhotos"
		' the DEVhotoImportFileLocation = "\\bbhcifs01\clientdata\staging\21195D\ChildPhotos"
		'Const _host As String = "files.blackbaudhosting.com"
		'Const _username As String = "MarkSe21195D@BBEC"
		'Const _password As String = "P@ssword2"
		'Const _workingdirectory As String = "/staging/21195D/ChildPhotos"
		'Const _uploadfile As String = "C:\OCM\PhotoImports\2013 Photos_Processing_6_25_2013\_Import\C210608.jpg"
		'Source Folder default location: C:\OCM\PhotoImports

		' This will use an account specially purposed for these SFTP transfers:  OcMFTPUser21195D@BBEC and OcMFTPUser21195P@BBEC

		'Dim ftpEnvironment As String = ConfigurationManager.AppSettings("FTPEnvironment")  'DEV, or PROD
		Dim ftpEnvironment As String = My.Settings.FTPEnvironment	  'DEV, or PROD
		If ftpEnvironment.ToLower().Equals("dev") Then
			'_ftpCRMPhotoFileLocation = ConfigurationManager.AppSettings("DEV_BlackbaudPhotoFileLocation")
			_ftpCRMPhotoFileLocation = My.Settings.DEV_BlackbaudPhotoFileLocation
			_username = My.Settings.DEV_FTPUserName
			_host = My.Settings.DEV_FTPHostName
			_workingdirectory = My.Settings.DEV_FTPWorkingDirectory
			_password = "P@ssword1"
		Else
			_ftpCRMPhotoFileLocation = My.Settings.PROD_BlackbaudPhotoFileLocation
			_username = My.Settings.PROD_FTPUserName
			_host = My.Settings.PROD_FTPHostName
			_workingdirectory = My.Settings.PROD_FTPWorkingDirectory
			_password = "P@ssword1"
		End If
		' this setting is same no matter what environment:
		_ftpImportFileLocation = My.Settings.ImportFileLocation
		_defaultSourceFolder = My.Settings.DefaultSourceFolder

		'set the label to identify the current environment:
		Me.lblEnvironment.Text = String.Format("will transfer to {0}", ftpEnvironment)

		'Dim output As String = String.Format("ftpEnvironment: {0} _ftpCRMPhotoFileLocation: {1} _username: {2} _workingdirectory: {3} _ftpImportFileLocation: {4}", ftpEnvironment, _ftpCRMPhotoFileLocation, _username, _workingdirectory, _ftpImportFileLocation)
		'MessageBox.Show(output)

	End Sub


End Class
