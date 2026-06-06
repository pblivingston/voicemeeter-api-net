// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

using PBLivingston.VoicemeeterAPI.Types;
using PBLivingston.VoicemeeterAPI.Utilities;
using Microsoft.Extensions.Logging;

public partial class Remote
{
    #region Is Parameters Dirty

    /// <inheritdoc/>
    internal bool Query_ParamsDirty()
    {
        this.LoginGuard();

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

        return this.Query_ParamsDirty();
    }

    #endregion

    #region Get Parameter Float

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    internal void GetParam_Float(string param, out float value)
    {
        this.LoginGuard();

        this.On_GetParam_Start(param);

        (var result, value) = this.wrapper.GetParameter_Float(param);

        if (result != Response.Ok)
        {
            throw this.On_GetParam_Error(result, param, value, typeof(float));
        }
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out float value)
    {
        using var scope = this.BeginInstanceScope();

        this.GetParam_Float(param, out value);

        this.On_GetParam_Success(param, value);
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out int value)
    {
        using var scope = this.BeginInstanceScope();

        this.GetParam_Float(param, out var val);

        value = Convert.ToInt32(val);

        if (Math.Abs(val - value) > 0.0001f || value < 0)
        {
            throw this.On_GetParam_Error(Response.TypeMismatch, param, val, typeof(int));
        }

        this.On_GetParam_Success(param, value);
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out bool value)
    {
        using var scope = this.BeginInstanceScope();

        this.GetParam_Float(param, out var val);

        var v = Convert.ToInt32(val);

        if (Math.Abs(val - v) > 0.0001f || v is not (0 or 1))
        {
            throw this.On_GetParam_Error(Response.TypeMismatch, param, val, typeof(bool));
        }

        value = v == 1;

        this.On_GetParam_Success(param, value);
    }

    #endregion

    #region Get Parameter String

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    internal void GetParam_String(string param, out string value)
    {
        this.LoginGuard();

        this.On_GetParam_Start(param);

        (var result, value) = this.wrapper.GetParameter_String(param);

        if (result != Response.Ok)
        {
            throw this.On_GetParam_Error(result, param, value, typeof(string));
        }

        this.On_GetParam_Success(param, value);
    }

    /// <inheritdoc cref="IRemote.GetParam{T}(string, out T)"/>
    public void GetParam(string param, out string value)
    {
        using var scope = this.BeginInstanceScope();

        this.GetParam_String(param, out value);
    }

    #endregion

    /// <inheritdoc/>
    void IRemote.GetParam<T>(string param, out T value)
    {
        if (typeof(T) == typeof(float))
        {
            this.GetParam(param, out float val);
            value = (T)(object)val;
            return;
        }

        if (typeof(T) == typeof(int))
        {
            this.GetParam(param, out int val);
            value = (T)(object)val;
            return;
        }

        if (typeof(T) == typeof(bool))
        {
            this.GetParam(param, out bool val);
            value = (T)(object)val;
            return;
        }

        if (typeof(T) == typeof(string))
        {
            this.GetParam(param, out string val);
            value = (T)(object)val;
            return;
        }

        throw GeneralDispatch.On_TypeNotSupported(this.logger, typeof(T), nameof(value), SupportedTypes.ParamTypes);
    }
}
