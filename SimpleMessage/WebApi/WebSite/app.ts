/// <reference path="../Scripts/typings/jquery/jquery.d.ts" />

class SmsClient {
    output: JQuery;

    constructor(output: JQuery) {
        this.output = output;
    }

    send(id: string, message: string) {
        var url = "api/sms/" + encodeURIComponent(id) + "?" + $.param({ message: message });
        $.ajax({
            url: url,
            method: 'POST',
            success: (_) => {
                this.writeOutput("Sent \"" + message + "\" to " + id);
            },
            error: (error) => {
                this.writeOutput("Error sending message: " + JSON.stringify(error));
            }
    });
    }

    get(id: string) {
        var url = "api/sms/" + encodeURIComponent(id);
        $.getJSON(url, (messages) => {
            this.writeOutput("Got " + messages.length + " messages for " + id + ":");
            for (var index in messages) {
                this.writeOutput("[" + index + "]: " + messages[index]);
            }

        });
    }

    private writeOutput(message: string) {
        this.output.append($('<span>').text(message));
        this.output.append($('<br>'));
    }
};

var output = $('#output');
var id = $('#id');
var msg = $('#msg');

var sms = new SmsClient(output);

$('#get').click((_) => {
    sms.get(id.val());
});


$('#send').click((_) => {
    sms.send(id.val(), msg.val());
});

$('#clear').click((_) => {
    output.html('');
});