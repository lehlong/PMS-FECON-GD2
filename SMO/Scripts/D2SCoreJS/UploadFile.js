window.UploadFile = {
    ListFile: [],
    ElementPreviewFile: "#divPreviewFile",
    FileGroupDefault: "xxxxxx",
    // Tham số elementPreviewFile : xác định nội dung preview file sẽ vào phần tử html nào
    // Tham số fileGroup : nhóm các file vào một nhóm fileGroup 
    InsertFile: function (elementPreviewFile, fileGroup) {
        if (elementPreviewFile != undefined && elementPreviewFile != null && elementPreviewFile != "") {
            UploadFile.ElementPreviewFile = elementPreviewFile;
        }

        if (fileGroup != undefined && fileGroup != null && fileGroup != "") {
            UploadFile.FileGroupDefault = fileGroup;
        }

        var name = "file" + Math.floor((Math.random() * 1000) + 1);
        var inputFile = $("<input type='file' id='files' name='" + name + "' multiple onchange='UploadFile.PreviewFile(this,\"" + UploadFile.FileGroupDefault + "\")'/>");
        $(inputFile).hide();
        $("body").append(inputFile);
        $(inputFile).click();
    },

    PreviewFile: function (obj, fileGroup) {
        for(var i=0; i < obj.files.length; i++){
            var file = obj.files[i];
            var fileExt = file.name.split('.').pop();
            var fileIcon = UploadFile.GetIcon(fileExt);
            var fileId = Math.floor((Math.random() * 1000) + 1);
            var preview = '<div class="preview-file preview-file-remove" onclick = "UploadFile.RemoveFile(this)" data-fileid = "' + fileId + '">'
            preview += '<img src = "\\Content\\IconFileType\\' + fileIcon + '" />'
            preview += '<div class="file-info">';
            preview += '<div class="file-name">' + file.name + "</div>"
            preview += '<div class="file-size">' + UploadFile.FormatByte(file.size,2) + "</div>"
            preview += '</div>';
            preview += '</div>';
            $(UploadFile.ElementPreviewFile).append(preview);
            UploadFile.ListFile.push({ FileId: fileId, DataFile: file, FileGroup: fileGroup });
        }
    },

    FormatByte: function (bytes, decimals) {
        if (bytes == 0) {
            return "0 Byte";
        }
        var k = 1024; //Or 1 kilo = 1000
        var sizes = ["Bytes", "KB", "MB", "GB", "TB", "PB"];
        var i = Math.floor(Math.log(bytes) / Math.log(k));
        return parseFloat((bytes / Math.pow(k, i)).toFixed(decimals)) + " " + sizes[i];

        //if(bytes < 1024) return bytes + " Bytes";
        //else if(bytes < 1048576) return(bytes / 1024).toFixed(3) + " KB";
        //else if(bytes < 1073741824) return(bytes / 1048576).toFixed(3) + " MB";
        //else return(bytes / 1073741824).toFixed(3) + " GB";
    },

    GetIcon: function (fileType) {
        var icon = "file-48.png";
        switch (fileType) {
            case "xlsx":
                icon = "excel-48.png";
                break;
            case "xls":
                icon = "excel-48.png";
                break;
            case "docx":
                icon = "word-48.png";
                break;
            case "doc":
                icon = "word-48.png";
                break;
            case "pptx":
                icon = "powerpoint-48.png";
                break;
            case "ppt":
                icon = "powerpoint-48.png";
                break;

            case "txt":
                icon = "txt-48.png";
                break;

            case "zip":
                icon = "zip-48.png";
                break;
            case "7z":
                icon = "zip-48.png";
                break;
            case "rar":
                icon = "zip-48.png";
                break;

            case "jpg":
                icon = "image-48.png";
                break;
            case "png":
                icon = "image-48.png";
                break;
            case "bmp":
                icon = "image-48.png";
                break;
            case "jpeg":
                icon = "image-48.png";
                break;

            case "psd":
                icon = "photoshop-48.png";
                break;
            case "dwg":
                icon = "dwg-48.png";
                break;
            case "dxf":
                icon = "dxf-48.png";
                break;

            case "pdf":
                icon = "pdf-48.png";
                break;

            default:
                break;
        }
        return icon;
    },

    RemoveFile: function (obj) {
        if (!confirm("Bạn có chắc chắn xóa file này?")) {
            return;
        }
        $(obj).remove();
        var fileId = $(obj).data("fileid");

        for (var i = 0; i < UploadFile.ListFile.length; ++i) {
            if (UploadFile.ListFile[i].FileId === fileId)
                UploadFile.ListFile.splice(i, 1);
        }
    },

    ClearFile: function (fileGroup) {
        if (fileGroup != undefined && fileGroup != null && fileGroup != "") {
            //UploadFile.ListFile.remove(function (item) {
            //    return item.FileGroup == fileGroup;
            //});
            UploadFile.ListFile = UploadFile.ListFile.filter(v => v.FileGroup !== fileGroup);
        }
        else {
            UploadFile.ListFile = [];
        }
    }
}