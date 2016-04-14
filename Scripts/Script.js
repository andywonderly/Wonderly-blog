$(document).ready(function() {

    $('.postBody').each(function () {

        //Desired initial body length
        var bodyLength = 250;
        var tooLong = false;

        var content = $(this).html();

        if (content.length > bodyLength) {
            this.innerHTML = content.substring(0, bodyLength) + " ...";
            tooLong = true;
        }
        else
        {
        this.innerHTML = content;
        }

    });
  

});