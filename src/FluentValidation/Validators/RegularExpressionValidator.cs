#region License
// Copyright (c) Jeremy Skinner (http://www.jeremyskinner.co.uk)
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at https://github.com/jeremyskinner/FluentValidation
#endregion

namespace FluentValidation.Validators {
	using System;
	using System.Text.RegularExpressions;
	using Attributes;
	using Internal;
	using Resources;
	using Results;

	public class RegularExpressionValidator : PropertyValidator, IRegularExpressionValidator {
		readonly string expression;
		readonly RegexOptions regexOptions = RegexOptions.None;
		readonly Func<object, string> expressionFunc;
		readonly Func<object, Regex> regexFunc;

		public RegularExpressionValidator(string expression) : base(nameof(Messages.regex_error), typeof(Messages)) {
			this.expression = expression;
		}

		public RegularExpressionValidator(Regex regex) : base(nameof(Messages.regex_error), typeof(Messages)) {
			this.expression = regex.ToString();
			this.regexFunc = x => regex;
		}

		public RegularExpressionValidator(string expression, RegexOptions options) : base(nameof(Messages.regex_error), typeof(Messages)) {
			this.expression = expression;
			this.regexOptions = options;
		}

		public RegularExpressionValidator(Func<object, string> expression)
			: base(nameof(Messages.regex_error), typeof(Messages)) {
			this.expressionFunc = expression;
		}

		public RegularExpressionValidator(Func<object, Regex> regex)
			: base(nameof(Messages.regex_error), typeof(Messages)) {
			this.regexFunc = regex;
		}

		public RegularExpressionValidator(Func<object, string> expression, RegexOptions options)
			: base(nameof(Messages.regex_error), typeof(Messages)) {
			this.expressionFunc = expression;
			this.regexOptions = options;
		}

		protected override bool IsValid(PropertyValidatorContext context) {
			Regex regex = null;

			if (regexFunc != null) {
				regex = regexFunc(context.Instance);
			}
			else if (expressionFunc != null) {
				regex = new Regex(expressionFunc(context.Instance), regexOptions);
			}
			else {
				regex = new Regex(expression, regexOptions);
			}


			if (context.PropertyValue != null && !regex.IsMatch((string) context.PropertyValue)) {
				context.MessageFormatter.AppendArgument("RegularExpression", regex.ToString());
				return false;
			}
			return true;
		}

		public string Expression {
			get { return expression; }
		}
	}

	public interface IRegularExpressionValidator : IPropertyValidator {
		string Expression { get; }
	}
}