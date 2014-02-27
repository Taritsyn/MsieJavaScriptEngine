/*!
* This polyfill based on the code of Douglas Crockford's ECMAScript 5 Polyfill v0.1
* (http://nuget.org/packages/ES5)
*/

// Function.prototype.bind
if (!Function.prototype.hasOwnProperty('bind')) {
	Function.prototype.bind = function (thisArg) {
		var slice = Array.prototype.slice,
			func = this,
			args = slice.call(arguments, 1)
			;

		return function () {
			return func.apply(thisArg, args.concat(slice.call(arguments, 0)));
		};
	};
}

// String.prototype.trim
if (!String.prototype.hasOwnProperty('trim')) {
	String.prototype.trim = (function (re) {
		return function () {
			return this.replace(re, "$1");
		};
	} (/^\s*([\s\S]*\S)?\s*$/));
}

// Array.prototype.every
if (!Array.prototype.hasOwnProperty('every')) {
	Array.prototype.every = function (callbackfn, thisArg) {
		var arr = this,
			i,
			length = arr.length
			;

		if (typeof callbackfn !== 'function') {
			throw new TypeError('Array.prototype.every: argument is not a Function object');
		}

		for (i = 0; i < length; i += 1) {
			if (arr.hasOwnProperty(i) && !callbackfn.call(thisArg, arr[i], i, arr)) {
				return false;
			}
		}

		return true;
	};
}

// Array.prototype.some
if (!Array.prototype.hasOwnProperty('some')) {
	Array.prototype.some = function (callbackfn, thisArg) {
		var arr = this,
			i,
			length = arr.length
			;

		if (typeof callbackfn !== 'function') {
			throw new TypeError('Array.prototype.some: argument is not a Function object');
		}

		for (i = 0; i < length; i += 1) {
			if (arr.hasOwnProperty(i) && callbackfn.call(thisArg, arr[i], i, arr)) {
				return true;
			}
		}

		return false;
	};
}

// Array.prototype.filter
if (!Array.prototype.hasOwnProperty('filter')) {
	Array.prototype.filter = function (callbackfn, thisArg) {
		var arr = this,
			i,
			length = arr.length,
			result = [],
			value
			;

		if (typeof callbackfn !== 'function') {
			throw new TypeError('Array.prototype.filter: argument is not a Function object');
		}

		for (i = 0; i < length; i += 1) {
			if (arr.hasOwnProperty(i)) {
				value = arr[i];

				if (callbackfn.call(thisArg, value, i, arr)) {
					result.push(value);
				}
			}
		}

		return result;
	};
}

// Array.prototype.forEach
if (!Array.prototype.hasOwnProperty('forEach')) {
	Array.prototype.forEach = function (callbackfn, thisArg) {
		var arr = this,
			i,
			length = arr.length
			;

		if (typeof callbackfn !== 'function') {
			throw new TypeError('Array.prototype.forEach: argument is not a Function object');
		}

		for (i = 0; i < length; i += 1) {
			if (arr.hasOwnProperty(i)) {
				callbackfn.call(thisArg, arr[i], i, arr);
			}
		}
	};
}

// Array.prototype.indexOf
if (!Array.prototype.hasOwnProperty('indexOf')) {
	Array.prototype.indexOf = function (searchElement, fromIndex) {
		var arr = this,
			i = fromIndex,
			length = arr.length
			;

		if (length === 0) {
			return -1;
		}

		if (typeof i !== 'number') {
			i = 0;
		}
		else if (i >= length) {
			return -1;
		}
		else if (i < 0) {
			i = length - Math.abs(i);
		}

		while (i < length) {
			if (arr.hasOwnProperty(i) && arr[i] === searchElement) {
				return i;
			}

			i += 1;
		}

		return -1;
	};
}

// Array.prototype.lastIndexOf
if (!Array.prototype.hasOwnProperty('lastIndexOf')) {
	Array.prototype.lastIndexOf = function (searchElement, fromIndex) {
		var arr = this,
			i = fromIndex,
			length = arr.length
			;

		if (length === 0) {
			return -1;
		}

		if (typeof i !== 'number') {
			i = length - 1;
		}
		else if (i >= length) {
			return -1;
		}
		else if (i < 0) {
			i = length - Math.abs(i);
		}

		while (i >= 0) {
			if (arr.hasOwnProperty(i) && arr[i] === searchElement) {
				return i;
			}

			i -= 1;
		}

		return -1;
	};
}

// Array.prototype.map
if (!Array.prototype.hasOwnProperty('map')) {
	Array.prototype.map = function (callbackfn, thisArg) {
		var arr = this,
			i,
			length = arr.length,
			result
			;

		if (typeof callbackfn !== 'function') {
			throw new TypeError('Array.prototype.map: argument is not a Function object');
		}

		result = new Array(length);

		for (i = 0; i < length; i += 1) {
			if (arr.hasOwnProperty(i)) {
				result[i] = callbackfn.call(thisArg, arr[i], i, arr);
			}
		}

		return result;
	};
}

// Array.prototype.reduce
if (!Array.prototype.hasOwnProperty('reduce')) {
	Array.prototype.reduce = function (callbackfn, initialValue) {
		var arr = this,
			i,
			length = arr.length,
			value = null,
			isValueSet = false
			;

		if (typeof callbackfn !== 'function') {
			throw new TypeError('Array.prototype.reduce: argument is not a Function object');
		}

		if (arguments.length > 1) {
			value = initialValue;
			isValueSet = true;
		}

		for (i = 0; length > i; i += 1) {
			if (arr.hasOwnProperty(i)) {
				if (isValueSet) {
					value = callbackfn.call(undefined, value, arr[i], i, arr);
				}
				else {
					value = this[i];
					isValueSet = true;
				}
			}
		}

		if (!isValueSet) {
			throw new TypeError('Reduce of empty array with no initial value');
		}

		return value;
	};
}

// Array.prototype.reduceRight
if (!Array.prototype.hasOwnProperty('reduceRight')) {
	Array.prototype.reduceRight = function (callbackfn, initialValue) {
		var arr = this,
			i,
			length = arr.length,
			value = null,
			isValueSet = false
			;

		if (typeof callbackfn !== 'function') {
			throw new TypeError('Array.prototype.reduceRight: argument is not a Function object');
		}

		if (arguments.length > 1) {
			value = initialValue;
			isValueSet = true;
		}

		for (i = length - 1; i >= 0; i -= 1) {
			if (arr.hasOwnProperty(i)) {
				if (isValueSet) {
					value = callbackfn.call(undefined, value, arr[i], i, arr);
				}
				else {
					value = arr[i];
					isValueSet = true;
				}
			}
		}

		if (!isValueSet) {
			throw new TypeError('Reduce of empty array with no initial value');
		}

		return value;
	};
}

// Date.now()
if (!Date.hasOwnProperty('now')) {
	Date.now = function () {
		return (new Date()).getTime();
	};
}

// Date.prototype.toISOString
if (!Date.prototype.hasOwnProperty('toISOString')) {
	Date.prototype.toISOString = function () {
		function f(n) {
			return n < 10 ? '0' + n : n;
		}

		return this.getUTCFullYear() + '-' +
			f(this.getUTCMonth() + 1) + '-' +
			f(this.getUTCDate()) + 'T' +
			f(this.getUTCHours()) + ':' +
			f(this.getUTCMinutes()) + ':' +
			f(this.getUTCSeconds()) + '.' +
			(this.getUTCMilliseconds() / 1000).toFixed(3).slice(2, 5) +
			'Z'
			;
	};
}

// Array.isArray
if (!Array.hasOwnProperty('isArray')) {
	Array.isArray = function (arg) {
		return Object.prototype.toString.apply(arg) === '[object Array]';
	};
}

// Object.keys
if (!Object.hasOwnProperty('keys')) {
	Object.keys = function (obj) {
		var prop,
			result = []
			;

		if (obj === null || (typeof obj !== 'object' && typeof obj !== 'function')) {
			throw new TypeError('Object.keys: argument is not an Object');
		}

		for (prop in obj) {
			if (Object.prototype.hasOwnProperty.call(obj, prop)) {
				result.push(prop);
			}
		}

		return result;
	};
}

// Object.create
if (!Object.hasOwnProperty('create')) {
	Object.create = function (obj) {
		if (arguments.length > 1) {
			throw new Error('Object.create implementation only accepts one parameter.');
		}

		function F() { }
		F.prototype = obj;

		return new F();
	};
}