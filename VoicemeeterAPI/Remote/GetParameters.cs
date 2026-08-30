// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region Is Parameters Dirty

    /// <inheritdoc cref="IRemote.IsParamsDirty()"/>
    internal Result<Response, bool> ParamsDirty_i(string executionPath)
    {
        using var lk = this.pDirtyLock.EnterScope();

        var response = this.wrapper.IsParametersDirty();

        return this.HandleDirtyResponse(response, Utilities.BuildPath(executionPath));
    }

    /// <inheritdoc/>
    public Result<Response, bool> IsParamsDirty()
    {
        using var scope = this.BeginCallScope();

        var result = this.ParamsDirty_i(nameof(this.IsParamsDirty));

        this.OnParamsDirty(result);

        return result;
    }

    #endregion

    #region Get Parameter Float

    /// <inheritdoc cref="IRemote.GetParamFloat(string)"/>
    internal Result<Response, float> GetParamFloat_i(string param, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetParameter_Float), e, param: param, trace: true);

        (var response, var value) = this.wrapper.GetParameter_Float(param);

        return this.HandleGetParamResponse(response, param, value, e);
    }

    /// <inheritdoc/>
    public Result<Response, float> GetParamFloat(string param)
    {
        using var scope = this.BeginCallScope();

        return this.GetParamFloat_i(param, nameof(this.GetParamFloat));
    }

    /// <inheritdoc cref="IRemote.GetParamInt(string)"/>
    internal Result<Response, int> GetParamInt_i(string param, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        var result = this.GetParamFloat_i(param, e);

        if (result.IsFailure)
        {
            return result.Response;
        }

        var val = result.Value;
        var value = Convert.ToInt32(val);

        if (Math.Abs(val - value) > 0.0001f || value < 0)
        {
            throw this.CannotConvertToType<int>(param, val, e);
        }

        return (result.Response, value);
    }

    /// <inheritdoc/>
    public Result<Response, int> GetParamInt(string param)
    {
        using var scope = this.BeginCallScope();

        return this.GetParamInt_i(param, nameof(this.GetParamInt));
    }

    /// <inheritdoc cref="IRemote.GetParamBool(string)"/>
    internal Result<Response, bool> GetParamBool_i(string param, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        var result = this.GetParamFloat_i(param, e);

        if (result.IsFailure)
        {
            return result.Response;
        }

        var val = result.Value;
        var v = Convert.ToInt32(val);

        if (Math.Abs(val - v) > 0.0001f || v is not (0 or 1))
        {
            throw this.CannotConvertToType<bool>(param, val, e);
        }

        return (result.Response, v == 1);
    }

    /// <inheritdoc/>
    public Result<Response, bool> GetParamBool(string param)
    {
        using var scope = this.BeginCallScope();

        return this.GetParamBool_i(param, nameof(this.GetParamBool));
    }

    #endregion

    #region Get Parameter String

    /// <inheritdoc cref="IRemote.GetParamString(string)"/>
    internal Result<Response, string> GetParamString_i(string param, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetParameter_String), e, param: param, trace: true);

        (var response, var value) = this.wrapper.GetParameter_String(param);

        return this.HandleGetParamResponse(response, param, value, e);
    }

    /// <inheritdoc/>
    public Result<Response, string> GetParamString(string param)
    {
        using var scope = this.BeginCallScope();

        return this.GetParamString_i(param, nameof(this.GetParamString));
    }

    #endregion
}
