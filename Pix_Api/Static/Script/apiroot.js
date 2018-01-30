
//Change text on hover over.
var oldText;
var WelcomeHoverOn = function() {
    var header = document.getElementById("WelcomeHeader");
    oldText = header.innerHTML;
    header.innerHTML = "This Doesn't Do Anything! (Yet)";
}
var WelcomeHoverOff = function() {
        var header = document.getElementById("WelcomeHeader");
        header.innerHTML = oldText;
    }
