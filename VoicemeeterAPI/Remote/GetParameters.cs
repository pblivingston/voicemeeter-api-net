// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    #region Is Parameters Dirty

    /// <inheritdoc cref="IRemote.IsParamsDirty()"/>
    internal bool ParamsDirty_i()
    {
        var level = LogLevel.Trace;

        this.On_Query_Start(level);

        var result = this.wrapper.IsParametersDirty();

        switch (result)
        {
            case Response.Ok:
                this.On_Query_Success(Response.Ok, level);
                return false;

            case Response.Dirty:
                this.On_ParamsDirty(level);
                return true;

            default:
                throw this.On_Method_Error(result);
        }
    }

    /// <inheritdoc/>
    public bool IsParamsDirty()
    {
        using var scope = this.BeginInstanceScope();

        return this.ParamsDirty_i();
    }

    #endregion

    #region Get Parameter Float

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    internal float GetParam_iFloat(string param)
    {
        this.On_GetParam_Start(param);

        (var result, var value) = this.wrapper.GetParameter_Float(param);

        if (result != Response.Ok)
        {
            throw this.On_GetParam_Error(result, param, value, typeof(float));
        }

        return value;
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    public float GetParamFloat(string param)
    {
        using var scope = this.BeginInstanceScope();

        var value = this.GetParam_iFloat(param);

        this.On_GetParam_Success(param, value);

        return value;
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    public int GetParamInt(string param)
    {
        using var scope = this.BeginInstanceScope();

        var val = this.GetParam_iFloat(param);

        var value = Convert.ToInt32(val);

        if (Math.Abs(val - value) > 0.0001f || value < 0)
        {
            throw this.On_GetParam_Error(Response.TypeMismatch, param, val, typeof(int));
        }

        this.On_GetParam_Success(param, value);

        return value;
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    public bool GetParamBool(string param)
    {
        using var scope = this.BeginInstanceScope();

        var val = this.GetParam_iFloat(param);

        var v = Convert.ToInt32(val);

        if (Math.Abs(val - v) > 0.0001f || v is not (0 or 1))
        {
            throw this.On_GetParam_Error(Response.TypeMismatch, param, val, typeof(bool));
        }

        var value = v == 1;

        this.On_GetParam_Success(param, value);

        return value;
    }

    #endregion

    #region Get Parameter String

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    internal string GetParam_iString(string param)
    {
        this.On_GetParam_Start(param);

        (var result, var value) = this.wrapper.GetParameter_String(param);

        if (result != Response.Ok)
        {
            throw this.On_GetParam_Error(result, param, value, typeof(string));
        }

        this.On_GetParam_Success(param, value);

        return value;
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string)"/>
    public string GetParamString(string param)
    {
        using var scope = this.BeginInstanceScope();

        return this.GetParam_iString(param);
    }

    #endregion

    /// <inheritdoc/>
    T IRemote.GetParam<T>(string param)
    {
        if (typeof(T) == typeof(float))
        {
            return (T)(object)this.GetParamFloat(param);
        }

        if (typeof(T) == typeof(int))
        {
            return (T)(object)this.GetParamInt(param);
        }

        if (typeof(T) == typeof(bool))
        {
            return (T)(object)this.GetParamBool(param);
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)this.GetParamString(param);
        }

        throw GeneralDispatch.On_TypeNotSupported(this.logger, typeof(T), nameof(T), SupportedTypes.ParamTypes);
    }
}
