(function ($) {

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

    var firstName = function (name) {
        return name.split(' ')[0];
    }

    var getParameterByName = function (name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    var setRSVPMessage = function (rsvp) {
        rsvp = rsvp || {};
        rsvp.guests = rsvp.guests || [];


        //we will start the poor-mans databinding, not worth using anything fancy as this is
        //the only thing in the site we need to databind
        $('.attending-unknown-container').toggle(rsvp.attending === undefined || rsvp.attending === null);
        $('.attending-yes-container').toggle(rsvp.attending === true);
        $('.attending-no-container').toggle(rsvp.attending === false);

        var primaryGuestName = rsvp.guests.length >= 1 ? rsvp.guests[0] : '';
        var otherGuests = rsvp.guests.length >= 2 ? rsvp.guests.slice(1) : [];
        var otherGuestsFriendlyString = ''
        if (otherGuests.length === 0) {
            otherGuestsFriendlyString = ''
        }
        else if (otherGuests.length === 1) {
            otherGuestsFriendlyString = ' and ' + firstName(otherGuests[0]);
        }
        else if (otherGuests.length >= 2) {
            var allButLastGuest = otherGuests.slice(0, Math.max(0, otherGuests.length - 1));
            for (var i = 0; i < allButLastGuest.length; i++) {
                allButLastGuest[i] = firstName(allButLastGuest[i]);
            }
            otherGuestsFriendlyString = ', ' + allButLastGuest.join(', ');
            otherGuestsFriendlyString += ' and ' + firstName(otherGuests[otherGuests.length - 1]);
        }

        $('#nameInput').val(primaryGuestName);
        $('#guestInput').val(otherGuests.join(', '));

        var yesText = $('.attending-yes-container p').attr('data-text');
        yesText = stringFormat(yesText, firstName(primaryGuestName), otherGuestsFriendlyString);
        $('.attending-yes-container p').html(yesText);

        var noText = $('.attending-no-container p').attr('data-text');
        noText = stringFormat(noText, firstName(primaryGuestName), otherGuestsFriendlyString);
        $('.attending-no-container p').html(noText);
    }



    $('#rsvpForm').submit(function (e) {
        e.preventDefault();
        var guests = [$('#nameInput').val()];
        guests.push.apply(guests, $('#guestInput').val().split(','));

        var guestsClean = []
        for (var i = 0; i < guests.length; i++) {
            var trimmed = guests[i].trim();
            if (trimmed)
                guestsClean.push(trimmed);
        }

        var rsvp = {
            attending: !!$("input:radio[name='attending']:checked").val(),
            guests: guestsClean
        }

        var tag = $('#tagInput').val();
        var isUpdate = tag ? true :false;

        var submitButton = $('button[type=submit]', this);
        var l = submitButton.ladda();
        l.ladda('start');
        
        var delayedCall = minDelayResponse($.ajax({
            type: isUpdate ? 'PUT' : 'POST',
            url: '/api/rsvp/' + (isUpdate ? '' : tag),
            data: JSON.stringify(rsvp),
            contentType: "application/json",
            dataType: 'json'
        }), 1000)

        delayedCall.done(function (data) {
            setRSVPMessage(data);
        }).fail(function (data) {
             alert('Something has gone wrong :(')
        }).always(function () {
            l.ladda('stop');
        });;
    });

    

    var setSendEnabledness = function () {
        $('#sendButton').prop('disabled', $('#nameInput').val().length == 0 || !$("input:radio[name='attending']").is(":checked"));
    }

    $("#nameInput").on('keyup', setSendEnabledness);
    $("input:radio[name='attending']").on('change', setSendEnabledness);


    //Run on startup
    (function () {
        var tag = getParameterByName("tag");

        if (tag) {
            $('#tagInput').val(tag);
            $.getJSON('/api/rsvp/' + tag, function (data, textStatus) {
                debugger;
                setRSVPMessage(data);
            }).error(function () {
                setRSVPMessage();
            });
        }
        else {
            setRSVPMessage()
        }
    })()

})(jQuery);