jQuery(function ($) {
    var wall = new freewall('#freewall');
    wall.reset({
        selector: 'li',
        animate: false,
        cellW: 150,
        cellH: 150,
        onResize: function () {
            wall.fitWidth();
        }
    });

    var appendInstagramImages = function (response) {
        var itemsToAppend = [];
        $.each(response.data, function (i, item) {
            var imageTypes = ['thumbnail', 'low_resolution', 'standard_resolution'];
            var rand = Math.floor(Math.random() * 2);
            var imageType = imageTypes[rand]
            var image = item.images[imageType];
            itemsToAppend.push('<li class="' + imageType + '"><a href="' + item.link + '" target="_blank"><img src="' + image.url + '" width="100%"/></a></li>')

        });
        wall.fitWidth();
        wall.appendBlock(itemsToAppend.join(''));


        var nextUrl = response.pagination.next_url;
        var $nextbutton = $('.load-more-images');
        $nextbutton.prop('disabled', false);
        if (nextUrl)
            $nextbutton.data('next-url', nextUrl);
        else
            $nextbutton.hide();
    }



    $('.load-more-images').click(function () {
        var nextUrl = $(this).data('next-url');
        if (nextUrl) {
            nextUrl = nextUrl.replace(/callback=.*?&/, 'callback=?&');
            $(this).prop('disabled', true);
            $.getJSON(nextUrl, appendInstagramImages);
        }
    })

    $.getJSON(instagramUrl, appendInstagramImages);

});