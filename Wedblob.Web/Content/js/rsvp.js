jQuery(function ($) {

    $.fn.animateHeight = function (heightChangeFn, animationTime) {

        this.each(function () {
            var $this = $(this);
            $this.data('startHeight', $this.height());
        });


        var elementsToShow = []
        var elementsToHide = []
        var showFn = function (element) {
            $element = element instanceof $ ? element : $(element);
            $element.each(function () {
                elementsToShow.push(this);

                //if we are showing an item we previous hid we will remove the item from being hidden
                var hideIndex = elementsToHide.indexOf(this);
                if (hideIndex >= 0) {
                    elementsToHide.splice(hideIndex, 1);
                }
            });
        }
        var hideFn = function (element) {
            $element = element instanceof $ ? element : $(element);
            $element.each(function () {
                elementsToHide.push(this);

                //if we are hiding an item we previous shown we will remove the item from being shown
                var showIndex = elementsToShow.indexOf(this);
                if (showIndex >= 0) {
                    elementsToShow.splice(showIndex, 1);
                }
            });
        }

        heightChangeFn(showFn, hideFn);

        var $elementsToHide = $(elementsToHide).filter(':visible');
        var $elementsToShow = $(elementsToShow).not(':visible');

        //set the elements to the final state so we can calculate the height
        $elementsToHide.hide();
        $elementsToShow.show();

        this.each(function () {
            var $this = $(this);
            var startHeight = $this.data('startHeight') || 0;

            $this.css({ height: 'auto' });
            var newHeight = $this.height();

            $this.css({ height: startHeight });
            $this.animate({ height: newHeight }, animationTime, function () {
                $this.css({ height: 'auto' })
            });
        });

        //set them back to their original states
        $elementsToShow.hide();
        $elementsToHide.show();

        //animate our elements to hide opacity from 1->0
        $elementsToHide.css({ opacity: 1 }).animate({ opacity: 0 }, animationTime / 2, function () {
            $elementsToHide.hide().css({ opacity: 1 });

            //animate elements to show opacity from 0->1
            $elementsToShow.show().css({ opacity: 0 }).animate({ opacity: 1 }, animationTime / 2);
        })

        if ($elementsToHide.length == 0) {
            $elementsToShow.show().css({ opacity: 0 }).animate({ opacity: 1 }, animationTime / 2);
        }


    };

    var stringFormat = function (format) {
        if (!format)
            return format;
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
              ? args[number]
              : match
            ;
        });
    };

    var minDelayResponse = function (promise, minDelay) {
        var deferred = $.Deferred();
        var start = new Date().getTime();
        promise.done(function () {
            var loadingTime = new Date().getTime() - start;
            var timeout = Math.max(0, 1000 - loadingTime);
            var self = this;
            var args = arguments;
            setTimeout(function () {
                deferred.resolveWith(self, args);
            }, timeout);
        }).fail(function () {
            var loadingTime = new Date().getTime() - start;
            var timeout = Math.max(0, 1000 - loadingTime);
            var self = this;
            var args = arguments;
            setTimeout(function () {
                deferred.rejectWith(self, args);
            }, timeout);
        });
        return deferred.promise();
    }

    var animateHeight = function () {

    }

    $('.search-form').submit(function (e) {
        e.preventDefault();
        var self = this;
        var searchQuery = $('.search-input', this).val();

        var submitButton = $('button[type=submit]', this);
        var l = submitButton.ladda();
        l.ladda('start');

        var delayedCall = minDelayResponse($.ajax({
            type: 'GET',
            url: '/api/rsvp/?q=' + encodeURIComponent(searchQuery),
            dataType: 'json'
        }), 1000);

        delayedCall.done(function (data) {
            var $searchResultContainer = $('.search-results').animateHeight(function (show, hide) {
                hide($('.search-results .header'));
                var headerClass = data.length > 0 ? '.matching' : '.no-matching';
                var $header = $('.search-results .header' + headerClass);
                var text = stringFormat($header.attr('data-text'), searchQuery);
                $header.html(text);
                show($header);

                $('.search-result-items').empty()
                    .jqoteapp('#resultItemTemplate', data)
                    .find('.guest-input')
                    .trigger('keyup');
            },250);
        }).fail(function (data) {
            alert('Something has gone wrong :(')
        }).always(function () {
            l.ladda('stop');
        });;
    });

    $('.search-form .search-input').on('keyup', function () {
        var $this = $(this);
        isValid = $(this).val().length >= 2;

        $this.closest('form').find('button[type="submit"], input[type="submit"]').prop('disabled', !isValid);
    }).trigger('keyup');

    //clicking on the rsvp button
    $('.search-results').on('click', '.detail-button', function () {
        var $this = $(this);
        $this.closest('ul').children('li').removeClass('detail');
        $this.closest('li').addClass('detail');
    })

    //enable our attending field only if the guest has a name
    $('.search-results').on('keyup', '.guest-input', function () {
        var $this = $(this);
        isValid = $(this).val().length > 0;

        var $select = $this.closest('.row').find('select');
        var wasDisabled = $select.prop('disabled');
        var willBeDisabled = !isValid;
        if (wasDisabled != willBeDisabled) {
            $select.val(isValid ? 'true' : 'false');
        }
        $select.prop('disabled', willBeDisabled);
    });


    //sending hte rsvps
    $('.search-results').on('click', '.send-button', function () {
        var $this = $(this);
        var $listItem = $this.closest('li');

        var l = $this.ladda();
        l.ladda('start');

        var disabledInput = $this.closest('li').find('.guest-row').find('input[type="text"]:enabled, select:enabled').prop('disabled', true);

        rsvps = []
        $listItem.find('.guest-row').each(function () {
            var $row = $(this);
            var $idHidden = $row.find('input[name="id"]');
            var $guestInput = $row.find('input[name="name"]');
            var $attendingInput = $row.find('select[name="attending"]');

            //only save rows that actually ahve an attending dropdown
            if ($attendingInput.length > 0) {
                var guestName = $row.find('input[name="name"]').val();

                //we are not attending if there is not guest specified, otherwise
                //we take the value of the drop down
                var attending = $guestInput.length === 1 && !guestName
                    ? false
                    : $attendingInput.val() == 'true';

                var rsvp = {
                    id : $idHidden.val(),
                    attending : attending
                };
                if ($guestInput.length === 1)
                    rsvp.name = guestName;
                rsvps.push(rsvp);
            }
        });

        console.log(rsvps);
       
        var delayedCall = minDelayResponse($.ajax({
            type: 'POST',
            url: '/api/rsvp',
            dataType: 'json',
            data: {
                guests: rsvps
            }
        }), 1000).done(function (data) {
            var updatedRowsHtml = $('#resultItemTemplate').jqote(data);
            var $updatedRows = $(updatedRowsHtml);
            $updatedRows.find('.footer-row').show().find(':input').prop('disabled',true);

            $updatedRows.addClass('detail')
                .find('.guest-input')
                .trigger('keyup');

            $listItem.replaceWith($updatedRows);

            $updatedRows.animateHeight(function (show, hide) {
                hide($updatedRows.find('.footer-row'));
            },100)

        }).fail(function (data) {
            alert('Something has gone wrong :(')
            disabledInput.prop('disabled', false);
        }).always(function () {
            
        });
     

    });

});