/// <reference path="../lib/jquery/dist/jquery.js" />

(function () {
	$("#selectLanguage select").change(function () {
		$(this).parents("form").submit();
	});
}());
