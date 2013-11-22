<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CRMPhotoImportHelper
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.lblNumProcessed = New System.Windows.Forms.Label()
		Me.lblNumProcessedValue = New System.Windows.Forms.Label()
		Me.txtSourceFolderName = New System.Windows.Forms.TextBox()
		Me.cmdProcessFolder = New System.Windows.Forms.Button()
		Me.rtbMessage = New System.Windows.Forms.RichTextBox()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.lblChildID = New System.Windows.Forms.Label()
		Me.cmdBrowseFolder = New System.Windows.Forms.Button()
		Me.cmdCancel = New System.Windows.Forms.Button()
		Me.cmdPause = New System.Windows.Forms.Button()
		Me.lblStatus = New System.Windows.Forms.Label()
		Me.lblNumInvalid = New System.Windows.Forms.Label()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.txtImportFileFolderName = New System.Windows.Forms.TextBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.cbPictureTitle = New System.Windows.Forms.ComboBox()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.txtBlackbaudPhotoLocation = New System.Windows.Forms.TextBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.txtMaxPhotos = New System.Windows.Forms.TextBox()
		Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
		Me.overallProgressBar = New System.Windows.Forms.ProgressBar()
		Me.progressBar1 = New System.Windows.Forms.ProgressBar()
		Me.btnTransfer = New System.Windows.Forms.Button()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.btnSelectFolder = New System.Windows.Forms.Button()
		Me.imageFilesSelectedLabel = New System.Windows.Forms.Label()
		Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
		Me.lblFilesInProgress = New System.Windows.Forms.Label()
		Me.lblEnvironment = New System.Windows.Forms.Label()
		Me.archiveCheckBox = New System.Windows.Forms.CheckBox()
		Me.unusableCheckBox = New System.Windows.Forms.CheckBox()
		Me.SuspendLayout()
		'
		'lblNumProcessed
		'
		Me.lblNumProcessed.AutoSize = True
		Me.lblNumProcessed.Location = New System.Drawing.Point(10, 36)
		Me.lblNumProcessed.Name = "lblNumProcessed"
		Me.lblNumProcessed.Size = New System.Drawing.Size(93, 13)
		Me.lblNumProcessed.TabIndex = 2
		Me.lblNumProcessed.Text = "Number Checked:"
		'
		'lblNumProcessedValue
		'
		Me.lblNumProcessedValue.AutoSize = True
		Me.lblNumProcessedValue.Location = New System.Drawing.Point(109, 36)
		Me.lblNumProcessedValue.Name = "lblNumProcessedValue"
		Me.lblNumProcessedValue.Size = New System.Drawing.Size(13, 13)
		Me.lblNumProcessedValue.TabIndex = 3
		Me.lblNumProcessedValue.Text = "0"
		'
		'txtSourceFolderName
		'
		Me.txtSourceFolderName.Location = New System.Drawing.Point(12, 337)
		Me.txtSourceFolderName.Name = "txtSourceFolderName"
		Me.txtSourceFolderName.Size = New System.Drawing.Size(534, 20)
		Me.txtSourceFolderName.TabIndex = 4
		'
		'cmdProcessFolder
		'
		Me.cmdProcessFolder.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.cmdProcessFolder.Location = New System.Drawing.Point(11, 363)
		Me.cmdProcessFolder.Name = "cmdProcessFolder"
		Me.cmdProcessFolder.Size = New System.Drawing.Size(130, 36)
		Me.cmdProcessFolder.TabIndex = 5
		Me.cmdProcessFolder.Text = "3. Process Folder"
		Me.cmdProcessFolder.UseVisualStyleBackColor = True
		'
		'rtbMessage
		'
		Me.rtbMessage.BackColor = System.Drawing.SystemColors.Control
		Me.rtbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.rtbMessage.ForeColor = System.Drawing.Color.Navy
		Me.rtbMessage.Location = New System.Drawing.Point(11, 94)
		Me.rtbMessage.Name = "rtbMessage"
		Me.rtbMessage.ReadOnly = True
		Me.rtbMessage.Size = New System.Drawing.Size(375, 146)
		Me.rtbMessage.TabIndex = 6
		Me.rtbMessage.Text = ""
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label1.Location = New System.Drawing.Point(11, 321)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(105, 13)
		Me.Label1.TabIndex = 7
		Me.Label1.Text = "2. Source Folder:"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(10, 14)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(61, 13)
		Me.Label2.TabIndex = 8
		Me.Label2.Text = "Last Photo:"
		'
		'lblChildID
		'
		Me.lblChildID.AutoSize = True
		Me.lblChildID.Location = New System.Drawing.Point(109, 14)
		Me.lblChildID.Name = "lblChildID"
		Me.lblChildID.Size = New System.Drawing.Size(41, 13)
		Me.lblChildID.TabIndex = 9
		Me.lblChildID.Text = "ChildID"
		'
		'cmdBrowseFolder
		'
		Me.cmdBrowseFolder.Location = New System.Drawing.Point(552, 335)
		Me.cmdBrowseFolder.Name = "cmdBrowseFolder"
		Me.cmdBrowseFolder.Size = New System.Drawing.Size(24, 22)
		Me.cmdBrowseFolder.TabIndex = 10
		Me.cmdBrowseFolder.Text = "..."
		Me.cmdBrowseFolder.UseVisualStyleBackColor = True
		'
		'cmdCancel
		'
		Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.cmdCancel.Location = New System.Drawing.Point(471, 516)
		Me.cmdCancel.Name = "cmdCancel"
		Me.cmdCancel.Size = New System.Drawing.Size(75, 23)
		Me.cmdCancel.TabIndex = 11
		Me.cmdCancel.Text = "Cancel"
		Me.cmdCancel.UseVisualStyleBackColor = True
		'
		'cmdPause
		'
		Me.cmdPause.Location = New System.Drawing.Point(386, 516)
		Me.cmdPause.Name = "cmdPause"
		Me.cmdPause.Size = New System.Drawing.Size(75, 23)
		Me.cmdPause.TabIndex = 12
		Me.cmdPause.Text = "Pause"
		Me.cmdPause.UseVisualStyleBackColor = True
		'
		'lblStatus
		'
		Me.lblStatus.AutoSize = True
		Me.lblStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.lblStatus.ForeColor = System.Drawing.Color.Maroon
		Me.lblStatus.Location = New System.Drawing.Point(10, 67)
		Me.lblStatus.Name = "lblStatus"
		Me.lblStatus.Size = New System.Drawing.Size(90, 15)
		Me.lblStatus.TabIndex = 13
		Me.lblStatus.Text = "Not Processing"
		'
		'lblNumInvalid
		'
		Me.lblNumInvalid.AutoSize = True
		Me.lblNumInvalid.Location = New System.Drawing.Point(330, 36)
		Me.lblNumInvalid.Name = "lblNumInvalid"
		Me.lblNumInvalid.Size = New System.Drawing.Size(13, 13)
		Me.lblNumInvalid.TabIndex = 15
		Me.lblNumInvalid.Text = "0"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(231, 36)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(81, 13)
		Me.Label4.TabIndex = 14
		Me.Label4.Text = "Number Invalid:"
		'
		'Button1
		'
		Me.Button1.Location = New System.Drawing.Point(551, 289)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(24, 22)
		Me.Button1.TabIndex = 19
		Me.Button1.Text = "..."
		Me.Button1.UseVisualStyleBackColor = True
		Me.Button1.Visible = False
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(456, 275)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(90, 13)
		Me.Label3.TabIndex = 18
		Me.Label3.Text = "Import File Folder:"
		Me.Label3.Visible = False
		'
		'txtImportFileFolderName
		'
		Me.txtImportFileFolderName.Location = New System.Drawing.Point(457, 291)
		Me.txtImportFileFolderName.Name = "txtImportFileFolderName"
		Me.txtImportFileFolderName.Size = New System.Drawing.Size(89, 20)
		Me.txtImportFileFolderName.TabIndex = 17
		Me.txtImportFileFolderName.Text = "<this field is hidden>"
		Me.txtImportFileFolderName.Visible = False
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label5.Location = New System.Drawing.Point(12, 267)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(95, 13)
		Me.Label5.TabIndex = 20
		Me.Label5.Text = "1. Picture Title:"
		'
		'cbPictureTitle
		'
		Me.cbPictureTitle.FormattingEnabled = True
		Me.cbPictureTitle.Items.AddRange(New Object() {"2012 Child Photo", "2012 Health Photo", "2013 Child Photo"})
		Me.cbPictureTitle.Location = New System.Drawing.Point(157, 264)
		Me.cbPictureTitle.Name = "cbPictureTitle"
		Me.cbPictureTitle.Size = New System.Drawing.Size(140, 21)
		Me.cbPictureTitle.TabIndex = 22
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(12, 241)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(139, 13)
		Me.Label6.TabIndex = 23
		Me.Label6.Text = "Blackbaud Photo Location: "
		Me.Label6.Visible = False
		'
		'txtBlackbaudPhotoLocation
		'
		Me.txtBlackbaudPhotoLocation.Location = New System.Drawing.Point(157, 238)
		Me.txtBlackbaudPhotoLocation.Name = "txtBlackbaudPhotoLocation"
		Me.txtBlackbaudPhotoLocation.Size = New System.Drawing.Size(389, 20)
		Me.txtBlackbaudPhotoLocation.TabIndex = 24
		Me.txtBlackbaudPhotoLocation.Text = "<this field is hidden>"
		Me.txtBlackbaudPhotoLocation.Visible = False
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(12, 294)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(119, 13)
		Me.Label7.TabIndex = 25
		Me.Label7.Text = "Max Photos Processed:"
		'
		'txtMaxPhotos
		'
		Me.txtMaxPhotos.Location = New System.Drawing.Point(157, 291)
		Me.txtMaxPhotos.Name = "txtMaxPhotos"
		Me.txtMaxPhotos.Size = New System.Drawing.Size(140, 20)
		Me.txtMaxPhotos.TabIndex = 26
		'
		'BackgroundWorker1
		'
		Me.BackgroundWorker1.WorkerReportsProgress = True
		Me.BackgroundWorker1.WorkerSupportsCancellation = True
		'
		'overallProgressBar
		'
		Me.overallProgressBar.Location = New System.Drawing.Point(248, 476)
		Me.overallProgressBar.Name = "overallProgressBar"
		Me.overallProgressBar.Size = New System.Drawing.Size(298, 23)
		Me.overallProgressBar.TabIndex = 27
		'
		'progressBar1
		'
		Me.progressBar1.Location = New System.Drawing.Point(11, 476)
		Me.progressBar1.Name = "progressBar1"
		Me.progressBar1.Size = New System.Drawing.Size(217, 23)
		Me.progressBar1.TabIndex = 28
		'
		'btnTransfer
		'
		Me.btnTransfer.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.btnTransfer.Location = New System.Drawing.Point(11, 405)
		Me.btnTransfer.Name = "btnTransfer"
		Me.btnTransfer.Size = New System.Drawing.Size(130, 36)
		Me.btnTransfer.TabIndex = 29
		Me.btnTransfer.Text = "4. Transfer Images"
		Me.btnTransfer.UseVisualStyleBackColor = True
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(8, 460)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(109, 13)
		Me.Label8.TabIndex = 30
		Me.Label8.Text = "Transfer File Progress"
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New System.Drawing.Point(245, 460)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(126, 13)
		Me.Label9.TabIndex = 31
		Me.Label9.Text = "Overall Transfer Progress"
		'
		'btnSelectFolder
		'
		Me.btnSelectFolder.Location = New System.Drawing.Point(157, 363)
		Me.btnSelectFolder.Name = "btnSelectFolder"
		Me.btnSelectFolder.Size = New System.Drawing.Size(130, 36)
		Me.btnSelectFolder.TabIndex = 32
		Me.btnSelectFolder.Text = "Select image files..."
		Me.btnSelectFolder.UseVisualStyleBackColor = True
		Me.btnSelectFolder.Visible = False
		'
		'imageFilesSelectedLabel
		'
		Me.imageFilesSelectedLabel.AutoSize = True
		Me.imageFilesSelectedLabel.Location = New System.Drawing.Point(154, 417)
		Me.imageFilesSelectedLabel.Name = "imageFilesSelectedLabel"
		Me.imageFilesSelectedLabel.Size = New System.Drawing.Size(149, 13)
		Me.imageFilesSelectedLabel.TabIndex = 33
		Me.imageFilesSelectedLabel.Text = "number of image files selected"
		'
		'OpenFileDialog1
		'
		Me.OpenFileDialog1.FileName = "OpenFileDialog1"
		'
		'lblFilesInProgress
		'
		Me.lblFilesInProgress.AutoSize = True
		Me.lblFilesInProgress.Location = New System.Drawing.Point(355, 417)
		Me.lblFilesInProgress.Name = "lblFilesInProgress"
		Me.lblFilesInProgress.Size = New System.Drawing.Size(115, 13)
		Me.lblFilesInProgress.TabIndex = 34
		Me.lblFilesInProgress.Text = "About to transfer files..."
		Me.lblFilesInProgress.Visible = False
		'
		'lblEnvironment
		'
		Me.lblEnvironment.AutoSize = True
		Me.lblEnvironment.Location = New System.Drawing.Point(468, 13)
		Me.lblEnvironment.Name = "lblEnvironment"
		Me.lblEnvironment.Size = New System.Drawing.Size(0, 13)
		Me.lblEnvironment.TabIndex = 35
		'
		'archiveCheckBox
		'
		Me.archiveCheckBox.AutoSize = True
		Me.archiveCheckBox.Location = New System.Drawing.Point(321, 264)
		Me.archiveCheckBox.Name = "archiveCheckBox"
		Me.archiveCheckBox.Size = New System.Drawing.Size(62, 17)
		Me.archiveCheckBox.TabIndex = 36
		Me.archiveCheckBox.Text = "Archive"
		Me.archiveCheckBox.UseVisualStyleBackColor = True
		'
		'unusableCheckBox
		'
		Me.unusableCheckBox.AutoSize = True
		Me.unusableCheckBox.Location = New System.Drawing.Point(321, 291)
		Me.unusableCheckBox.Name = "unusableCheckBox"
		Me.unusableCheckBox.Size = New System.Drawing.Size(71, 17)
		Me.unusableCheckBox.TabIndex = 37
		Me.unusableCheckBox.Text = "Unusable"
		Me.unusableCheckBox.UseVisualStyleBackColor = True
		'
		'CRMPhotoImportHelper
		'
		Me.AcceptButton = Me.cmdProcessFolder
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.cmdCancel
		Me.ClientSize = New System.Drawing.Size(589, 549)
		Me.Controls.Add(Me.unusableCheckBox)
		Me.Controls.Add(Me.archiveCheckBox)
		Me.Controls.Add(Me.lblEnvironment)
		Me.Controls.Add(Me.lblFilesInProgress)
		Me.Controls.Add(Me.imageFilesSelectedLabel)
		Me.Controls.Add(Me.btnSelectFolder)
		Me.Controls.Add(Me.Label9)
		Me.Controls.Add(Me.Label8)
		Me.Controls.Add(Me.btnTransfer)
		Me.Controls.Add(Me.progressBar1)
		Me.Controls.Add(Me.overallProgressBar)
		Me.Controls.Add(Me.txtMaxPhotos)
		Me.Controls.Add(Me.Label7)
		Me.Controls.Add(Me.txtBlackbaudPhotoLocation)
		Me.Controls.Add(Me.Label6)
		Me.Controls.Add(Me.cbPictureTitle)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.txtImportFileFolderName)
		Me.Controls.Add(Me.lblNumInvalid)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.lblStatus)
		Me.Controls.Add(Me.cmdPause)
		Me.Controls.Add(Me.cmdCancel)
		Me.Controls.Add(Me.cmdBrowseFolder)
		Me.Controls.Add(Me.lblChildID)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.rtbMessage)
		Me.Controls.Add(Me.cmdProcessFolder)
		Me.Controls.Add(Me.txtSourceFolderName)
		Me.Controls.Add(Me.lblNumProcessedValue)
		Me.Controls.Add(Me.lblNumProcessed)
		Me.Name = "CRMPhotoImportHelper"
		Me.Text = "CRM Photo Import Helper"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents lblNumProcessed As System.Windows.Forms.Label
	Friend WithEvents lblNumProcessedValue As System.Windows.Forms.Label
	Friend WithEvents txtSourceFolderName As System.Windows.Forms.TextBox
	Friend WithEvents cmdProcessFolder As System.Windows.Forms.Button
	Friend WithEvents rtbMessage As System.Windows.Forms.RichTextBox
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents lblChildID As System.Windows.Forms.Label
	Friend WithEvents cmdBrowseFolder As System.Windows.Forms.Button
	Friend WithEvents cmdCancel As System.Windows.Forms.Button
	Friend WithEvents cmdPause As System.Windows.Forms.Button
	Friend WithEvents lblStatus As System.Windows.Forms.Label
	Friend WithEvents lblNumInvalid As System.Windows.Forms.Label
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents Button1 As System.Windows.Forms.Button
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents txtImportFileFolderName As System.Windows.Forms.TextBox
	Friend WithEvents Label5 As System.Windows.Forms.Label
	Friend WithEvents cbPictureTitle As System.Windows.Forms.ComboBox
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents txtBlackbaudPhotoLocation As System.Windows.Forms.TextBox
	Friend WithEvents Label7 As System.Windows.Forms.Label
	Friend WithEvents txtMaxPhotos As System.Windows.Forms.TextBox
	Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
	Friend WithEvents overallProgressBar As System.Windows.Forms.ProgressBar
	Friend WithEvents progressBar1 As System.Windows.Forms.ProgressBar
	Friend WithEvents btnTransfer As System.Windows.Forms.Button
	Friend WithEvents Label8 As System.Windows.Forms.Label
	Friend WithEvents Label9 As System.Windows.Forms.Label
	Friend WithEvents btnSelectFolder As System.Windows.Forms.Button
	Friend WithEvents imageFilesSelectedLabel As System.Windows.Forms.Label
	Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
	Friend WithEvents lblFilesInProgress As System.Windows.Forms.Label
	Friend WithEvents lblEnvironment As System.Windows.Forms.Label
	Friend WithEvents archiveCheckBox As System.Windows.Forms.CheckBox
	Friend WithEvents unusableCheckBox As System.Windows.Forms.CheckBox

End Class
