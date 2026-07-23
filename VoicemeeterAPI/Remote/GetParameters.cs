// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

public partial class Remote
{
    #region Is Parameters Dirty

    /// <inheritdoc cref="IRemote.IsParamsDirty()"/>
    internal bool ParamsDirty_i(string executionPath)
    {
        Response result;
        using (this.pDirtyLock.EnterScope())
        {
            result = this.wrapper.IsParametersDirty();
        }

        return this.HandleResponse(result, Utilities.BuildPath(executionPath));
    }

    /// <inheritdoc/>
    public bool IsParamsDirty()
    {
        using var scope = this.BeginCallScope();

        return this.ParamsDirty_i(nameof(this.IsParamsDirty));
    }

    #endregion

    #region Get Parameter Float

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    internal float GetParamFloat_i(string param, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetParameter_Float), e, param: param, trace: true);

        (var result, var value) = this.wrapper.GetParameter_Float(param);

        this.HandleResponse(result, param, value, e);

        return value;
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    public float GetParamFloat(string param)
    {
        using var scope = this.BeginCallScope();

        return this.GetParamFloat_i(param, nameof(this.GetParamFloat));
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    internal int GetParamInt_i(string param, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        var val = this.GetParamFloat_i(param, e);

        var value = Convert.ToInt32(val);

        if (Math.Abs(val - value) > 0.0001f || value < 0)
        {
            throw this.CannotConvertToType<int>(param, val, e);
        }

        return value;
    }

    public int GetParamInt(string param)
    {
        using var scope = this.BeginCallScope();

        return this.GetParamInt_i(param, nameof(this.GetParamInt));
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    internal bool GetParamBool_i(string param, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        var val = this.GetParamFloat_i(param, e);

        var v = Convert.ToInt32(val);

        if (Math.Abs(val - v) > 0.0001f || v is not (0 or 1))
        {
            throw this.CannotConvertToType<bool>(param, val, e);
        }

        return v == 1;
    }

    public bool GetParamBool(string param)
    {
        using var scope = this.BeginCallScope();

        return this.GetParamBool_i(param, nameof(this.GetParamBool));
    }

    #endregion

    #region Get Parameter String

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    internal string GetParamString_i(string param, string executionPath)
    {
        var e = Utilities.BuildPath(executionPath);

        this.WrapperCall(nameof(this.wrapper.GetParameter_String), e, param: param, trace: true);

        (var result, var value) = this.wrapper.GetParameter_String(param);

        this.HandleResponse(result, param, value, e);

        return value;
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    public string GetParamString(string param)
    {
        using var scope = this.BeginCallScope();

        return this.GetParamString_i(param, nameof(this.GetParamString));
    }

    #endregion

    /// <inheritdoc/>
    T IRemote.GetParam<T>(string param)
    {
        var t = typeof(T);

        return t switch
        {
            _ when t == typeof(float) => (T)(object)this.GetParamFloat(param),
            _ when t == typeof(int) => (T)(object)this.GetParamInt(param),
            _ when t == typeof(bool) => (T)(object)this.GetParamBool(param),
            _ when t == typeof(string) => (T)(object)this.GetParamString(param),
            _ => throw this.TypeNotSupported<T>(SupportedTypes.ParamTypes, nameof(IRemote.GetParam))
        };
    }
}
