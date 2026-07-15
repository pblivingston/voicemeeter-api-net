// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Exceptions;

public class VmArgumentException : VmApiException
{
    public string? ParamName { get; }

    public VmArgumentException(string message, string paramName, Exception innerException)
        : base(message, innerException)
        => this.ParamName = paramName;

    public VmArgumentException(string message, string paramName)
        : base(message)
        => this.ParamName = paramName;

    public VmArgumentException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public VmArgumentException(string message)
        : base(message)
    { }

    public VmArgumentException()
        : base()
    { }

    public override string Message
    {
        get
        {
            var lines = new List<string>();

            if (!string.IsNullOrWhiteSpace(base.Message))
            {
                lines.Add(base.Message);
            }

            if (!string.IsNullOrEmpty(this.ParamName))
            {
                lines.Add($"Parameter name: {this.ParamName}");
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
