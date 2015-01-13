/*!
* This polyfill based on code of the following libraries:
* 1. Douglas Crockford's ECMAScript 5 Polyfill v0.1 - http://nuget.org/packages/ES5
* 2. MDN JavaScript Polyfills - http://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference
* 3. Steven Levithan's Cross-Browser Split v1.1.1 - http://blog.stevenlevithan.com/archives/cross-browser-split
*/

(function (undefined) {
	var ERROR_MSG_ARGUMENT_IS_NOT_A_OBJECT = 'argument is not an Object.',
		ERROR_MSG_ARGUMENT_IS_NOT_A_FUNCTION = 'argument is not a Function object.',
		ERROR_MSG_IMPLEMENTATION_ONLY_ACCEPTS_ONE_PARAMETER = 'implementation only accepts one parameter.',
		ERROR_MSG_REDUCE_OF_EMPTY_ARRAY_WITH_NO_INITIAL_VALUE = 'reduce of empty array with no initial value.',
		nativeStringSplit,
		isExecNpcgSupported // NPCG: nonparticipating capturing group
		;
		
	//#region Internal methods
	
	function isArray(arg) {
		return objectToString(arg) === '[object Array]';
	}
	
	function isNumber(arg) {
		return typeof arg === 'number';
	}

	function isUndefined(arg) {
		return arg === void 0;
	}

	function isRegExp(re) {
		return isObject(re) && objectToString(re) === '[object RegExp]';
	}

	function isObject(arg) {
		return typeof arg === 'object' && arg !== null;
	}

	function isFunction(arg) {
		return typeof arg === 'function';
	}

	function objectToString(obj) {
		return Object.prototype.toString.call(obj);
	}
	
	//#endregion

	//#region Array methods
	
	// Array.prototype.every
	if (!Array.prototype.hasOwnProperty('every')) {
		Array.prototype.every = function (callbackfn, thisArg) {
			var arr = this,
				i,
				length = arr.length
				;

			if (!isFunction(callbackfn)) {
				throw new TypeError('Array.prototype.every: ' + ERROR_MSG_ARGUMENT_IS_NOT_A_FUNCTION);
			}

			for (i = 0; i < length; i += 1) {
				if (arr.hasOwnProperty(i) && !callbackfn.call(thisArg, arr[i], i, arr)) {
					return false;
				}
			}

			return true;
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

			if (!isFunction(callbackfn)) {
				throw new TypeError('Array.prototype.filter: ' + ERROR_MSG_ARGUMENT_IS_NOT_A_FUNCTION);
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

			if (!isFunction(callbackfn)) {
				throw new TypeError('Array.prototype.forEach: ' + ERROR_MSG_ARGUMENT_IS_NOT_A_FUNCTION);
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

			if (!isNumber(i)) {
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
	
	// Array.isArray
	if (!Array.hasOwnProperty('isArray')) {
		Array.isArray = isArray;
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

			if (!isNumber(i)) {
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

			if (!isFunction(callbackfn)) {
				throw new TypeError('Array.prototype.map: ' + ERROR_MSG_ARGUMENT_IS_NOT_A_FUNCTION);
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
				isValueSet = false,
				methodName = 'Array.prototype.reduce'
				;

			if (!isFunction(callbackfn)) {
				throw new TypeError(methodName + ': ' + ERROR_MSG_ARGUMENT_IS_NOT_A_FUNCTION);
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
				throw new TypeError(methodName + ': ' + ERROR_MSG_REDUCE_OF_EMPTY_ARRAY_WITH_NO_INITIAL_VALUE);
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
				isValueSet = false,
				methodName = 'Array.prototype.reduceRight'
				;

			if (!isFunction(callbackfn)) {
				throw new TypeError(methodName + ': ' + ERROR_MSG_ARGUMENT_IS_NOT_A_FUNCTION);
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
				throw new TypeError(methodName + ': ' + ERROR_MSG_REDUCE_OF_EMPTY_ARRAY_WITH_NO_INITIAL_VALUE);
			}

			return value;
		};
	}

	// Array.prototype.some
	if (!Array.prototype.hasOwnProperty('some')) {
		Array.prototype.some = function (callbackfn, thisArg) {
			var arr = this,
				i,
				length = arr.length
				;

			if (!isFunction(callbackfn)) {
				throw new TypeError('Array.prototype.some: ' + ERROR_MSG_ARGUMENT_IS_NOT_A_FUNCTION);
			}

			for (i = 0; i < length; i += 1) {
				if (arr.hasOwnProperty(i) && callbackfn.call(thisArg, arr[i], i, arr)) {
					return true;
				}
			}

			return false;
		};
	}

	//#endregion

	//#region Date methods

	// Date.now
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

	//#endregion

	//#region Function methods
	
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
	
	//#endregion

	//#region Object methods
	
	// Object.create
	if (!Object.hasOwnProperty('create')) {
		Object.create = function (obj) {
			if (arguments.length > 1) {
				throw new Error('Object.create: ' + ERROR_MSG_IMPLEMENTATION_ONLY_ACCEPTS_ONE_PARAMETER);
			}

			function F() { }
			F.prototype = obj;

			return new F();
		};
	}
	
	// Object.keys
	if (!Object.hasOwnProperty('keys')) {
		Object.keys = function (obj) {
			var prop,
				result = []
				;

			if (!isObject(obj) && !isFunction(obj)) {
				throw new TypeError('Object.keys: ' + ERROR_MSG_ARGUMENT_IS_NOT_A_OBJECT);
			}

			for (prop in obj) {
				if (Object.prototype.hasOwnProperty.call(obj, prop)) {
					result.push(prop);
				}
			}

			return result;
		};
	}
	
	//#endregion

	//#region String methods
	
	// String.prototype.split
	if ('aa'.split(/a/).length === 0 ||
		'|a|'.split(/\|/).length === 1 ||
		'1, 2'.split(/\s*(,)/).length === 2) {
		
		nativeStringSplit = String.prototype.split;
		isExecNpcgSupported = isUndefined(/()??/.exec('')[1]);
		
		String.prototype.split = function (separator, limit) {
			var str = this,
				result,
				flags,
				lastLastIndex,
				separator1,
				separator2,
				match,
				lastIndex,
				lastLength,
				argIndex,
				argCount
				;
		
			// If `separator` is not a regex, use `nativeStringSplit`
			if (!isRegExp(separator)) {
				return nativeStringSplit.call(str, separator, limit);
			}

			result = [];
			flags = (separator.ignoreCase ? 'i' : '') +
				(separator.multiline ? 'm' : '') +
				(separator.extended ? 'x' : '') + // Proposed for ES6
				(separator.sticky ? 'y' : '') // Firefox 3+
				;
			lastLastIndex = 0;
			
			// Make `global` and avoid `lastIndex` issues by working with a copy
			separator1 = new RegExp(separator.source, flags + 'g');

			if (!isExecNpcgSupported) {
				// Doesn't need flags gy, but they don't hurt
				separator2 = new RegExp('^' + separator1.source + '$(?!\\s)', flags);
			}
			
			/* Values for `limit`, per the spec:
			 * If undefined: 4294967295 // Math.pow(2, 32) - 1
			 * If 0, Infinity, or NaN: 0
			 * If positive number: limit = Math.floor(limit); if (limit > 4294967295) limit -= 4294967296;
			 * If negative number: 4294967296 - Math.floor(Math.abs(limit))
			 * If other: Type-convert, then use the above rules
			 */
			limit = isUndefined(limit) ?
				-1 >>> 0 // Math.pow(2, 32) - 1
				:
				limit >>> 0 // ToUint32(limit)
				;

			while (match = separator1.exec(str)) {
				// `separator1.lastIndex` is not reliable cross-browser
				lastIndex = match.index + match[0].length;
				if (lastIndex > lastLastIndex) {
					result.push(str.slice(lastLastIndex, match.index));

					// Fix browsers whose `exec` methods don't consistently return `undefined` for
					// nonparticipating capturing groups
					if (!isExecNpcgSupported && match.length > 1) {
						match[0].replace(separator2, function () {
							for (argIndex = 1, argCount = arguments.length; argIndex < argCount - 2; argIndex++) {
								if (isUndefined(arguments[argIndex])) {
									match[argIndex] = undefined;
								}
							}
						});
					}

					if (match.length > 1 && match.index < str.length) {
						Array.prototype.push.apply(result, match.slice(1));
					}

					lastLength = match[0].length;
					lastLastIndex = lastIndex;

					if (result.length >= limit) {
						break;
					}
				}

				if (separator1.lastIndex === match.index) {
					separator1.lastIndex++; // Avoid an infinite loop
				}
			}

			if (lastLastIndex === str.length) {
				if (lastLength || !separator1.test('')) {
					result.push('');
				}
			}
			else {
				result.push(str.slice(lastLastIndex));
			}

			return (result.length > limit) ? result.slice(0, limit) : result;
		};
	}
	
	// String.prototype.trim
	if (!String.prototype.hasOwnProperty('trim')) {
		String.prototype.trim = (function (re) {
			return function () {
				return this.replace(re, '$1');
			};
		} (/^\s*([\s\S]*\S)?\s*$/));
	}
	
	//#endregion
}());