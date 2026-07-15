// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public class VmArgumentOutOfRangeException : VmArgumentException
{
    public object? ActualValue { get; }

    public VmArgumentOutOfRangeException(string message, string paramName, object? actualValue)
        : base(message, paramName)
        => this.ActualValue = actualValue;

    public VmArgumentOutOfRangeException(string message, string paramName)
        : base(message, paramName)
    { }

    public VmArgumentOutOfRangeException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public VmArgumentOutOfRangeException(string message)
        : base(message)
    { }

    public VmArgumentOutOfRangeException()
        : base()
    { }

    public override string Message
    {
        get
        {
            var lines = new List<string>();

            if (!string.IsNullOrEmpty(this.ParamName))
            {
                var paramMessage = $"'{this.ParamName}' was out of range.";

                if (!string.IsNullOrWhiteSpace(base.Message))
                {
                    paramMessage += " " + base.Message;
                }

                lines.Add(paramMessage);
            }
            else if (!string.IsNullOrWhiteSpace(base.Message))
            {
                lines.Add(base.Message);
            }

            if (this.ActualValue is not null)
            {
                lines.Add($"Actual Value: {this.ActualValue}");
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
