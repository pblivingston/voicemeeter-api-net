// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Internal;

#if NET6_0_OR_GREATER
using System.Globalization;
#endif

using System.Text;

internal static class StringBuilderExt
{
    public static StringBuilder AddArg<T>(this StringBuilder builder, string label, T value) where T : struct
    {
#if NET6_0_OR_GREATER
        builder.Append(CultureInfo.InvariantCulture, $"{label}: {value}; ");
#else
        builder.Append($"{label}: {value}; ");
#endif
        return builder;
    }

    public static StringBuilder AddNullableArg<T>(this StringBuilder builder, string label, T? value)
    {
        if (value is not null)
        {
#if NET6_0_OR_GREATER
            builder.Append(CultureInfo.InvariantCulture, $"{label}: {value}; ");
#else
            builder.Append($"{label}: {value}; ");
#endif
        }

        return builder;
    }
}

internal static class SemaphoreExt
{
    public static async Task<Scope> EnterScopeAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        return new Scope(semaphore);
    }

    public static Scope EnterScope(this SemaphoreSlim semaphore)
    {
        semaphore.Wait();
        return new Scope(semaphore);
    }

    public readonly struct Scope(SemaphoreSlim semaphore) : IDisposable
    {
        private readonly SemaphoreSlim semaphore = semaphore;
        public void Dispose()
            => this.semaphore?.Release();
    }
}
