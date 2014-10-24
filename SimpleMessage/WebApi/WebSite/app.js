/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />
var SmsClient = (function () {
    function SmsClient(output) {
        this.output = output;
    }
    SmsClient.prototype.send = function (id, message) {
        var _this = this;
        var url = "api/sms/" + encodeURIComponent(id) + "?" + $.param({ message: message });
        $.ajax({
            url: url,
            method: 'POST',
            success: function (_) {
                _this.writeOutput("Sent \"" + message + "\" to " + id);
            },
            error: function (error) {
                _this.writeOutput("Error sending message: " + JSON.stringify(error));
            }
        });
    };

    SmsClient.prototype.get = function (id) {
        var _this = this;
        var url = "api/sms/" + encodeURIComponent(id);
        $.getJSON(url, function (messages) {
            _this.writeOutput("Got " + messages.length + " messages for " + id + ":");
            for (var index in messages) {
                _this.writeOutput("[" + index + "]: " + messages[index]);
            }
        });
    };

    SmsClient.prototype.writeOutput = function (message) {
        this.output.append($('<span>').text(message));
        this.output.append($('<br>'));
    };
    return SmsClient;
})();
;

var output = $('#output');
var id = $('#id');
var msg = $('#msg');

var sms = new SmsClient(output);

$('#get').click(function (_) {
    sms.get(id.val());
});

$('#send').click(function (_) {
    sms.send(id.val(), msg.val());
});

$('#clear').click(function (_) {
    output.html('');
});
//# sourceMappingURL=app.js.map
