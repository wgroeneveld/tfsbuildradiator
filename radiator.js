var radiator = (function($) {

	var buildColors = {
		"Succeeded": "lightgreen",
		"PartiallySucceeded": "yellow",
		"Failed": "red"
	};

	function parse(project) {
		$("#builds").append(
			$("<div/>")
				.html("<h3>" + project.DefinitionName + "</h3><span class='time'>" + project.StartTime + "</span>")
				.css("background-color", buildColors[project.Status])
		);
	}

	function getBuildStatus(buildArray) {
		$("#debug").html("updating...");
		$("#builds").html("");
	
		$.getJSON("buildstatus.json", function(json) {
			json.Projects.forEach(function(project) {
				if(buildArray.indexOf(project.DefinitionName) != -1) {
					parse(project);
				}
			});
			
			$("#debug").html("");
		});	
	}

	return {
		getBuildStatus: function(buildArray) {
			$(getBuildStatus(buildArray));
		}
	};

})(jQuery);
