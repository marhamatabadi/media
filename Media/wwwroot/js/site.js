function onFolderClicked(e,folderId) {
    let title = $(`#span-title_${folderId}`).text();
    $(`#span-title_${folderId}`).addClass('d-none');
    $(`#input-title_${folderId}`).removeClass('d-none');
    $(`#input-title_${folderId}`).val(title);
}
function renameFolder(e) {
    let folderTitle = $(e).val();
    let folderId = parseInt($(e).attr('data-folderid'));

    $(e).prop('disabled', true);
    let data = {
        'folderTitle': folderTitle,
        'folderId': folderId
    };

    $.ajax('/Home/RenameFolder', {
        type: 'POST',
        data: data,
        success: function (data, status, xhr) {
            if (data.isSuccess === true) {
                $(`#span-title_${folderId}`).html(folderTitle);
                $(`#span-title_${folderId}`).removeClass('d-none');
                $(`#span-title_${folderId}`).addClass('d-block');

                $(`#input-title_${folderId}`).removeClass('d-block');
                $(`#input-title_${folderId}`).addClass('d-none');
            } else {
                alert(data.message);
            }
        },
        error: function (jqXhr, textStatus, errorMessage) {
            console.error({ jqXhr, textStatus, errorMessage })
        },
        complete: function () {
            $(e).prop('disabled', false);
        }
    });

    
}

$(document).ready(function () {
    $('.rename-folder').on('change', function () { renameFolder(this) });
})