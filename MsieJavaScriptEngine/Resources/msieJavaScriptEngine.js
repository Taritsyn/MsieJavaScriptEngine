var msieJavaScript = (function () {
	var setPropertyValue = function (obj, propertyName, value) {
		if (propertyName.indexOf(".") !== -1) {
			var parts = propertyName.split(".");
			var partCount = parts.length;
			var parent = obj;

			for (var partIndex = 0; partIndex < partCount; partIndex += 1) {
				var part = parts[partIndex];

				if (partIndex == (partCount - 1)) {
					parent[part] = value;
				}
				else {
					if (typeof parent[part] === "undefined") {
						parent[part] = {};
					}

					parent = parent[part];
				}
			}
		}
		else {
			obj[propertyName] = value;
		}
	};

	return { setPropertyValue: setPropertyValue };
})();