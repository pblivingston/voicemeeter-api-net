// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Types;

public interface IVersion<T> : IEquatable<T>, IComparable<T>, IVersion
    where T : struct, IVersion<T>
{
}

public interface IVersion : IComparable
{
    /// <summary>
    ///   <code>(kind &lt;&lt; 24) | (maj &lt;&lt; 16) | (min &lt;&lt; 8) | pat;</code>
    /// </summary>
    public int Packed { get; }

    /// <summary>
    ///   <code>(IVersion.Packed &gt;&gt; 24) &amp; 0xFF;</code>
    /// </summary>
    public int Kind { get; }

    /// <summary>
    ///   <code>(IVersion.Packed &gt;&gt; 16) &amp; 0xFF;</code>
    /// </summary>
    public int Major { get; }

    /// <summary>
    ///   <code>(IVersion.Packed &gt;&gt; 8) &amp; 0xFF;</code>
    /// </summary>
    public int Minor { get; }

    /// <summary>
    ///   <code>IVersion.Packed &amp; 0xFF;</code>
    /// </summary>
    public int Patch { get; }

    /// <summary>
    ///   <code>(Kind)((IVersion.Packed &gt;&gt; 24) &amp; 0xFF);</code>
    /// </summary>
    public Kind K { get; }

    /// <summary>
    ///   <code>(SemVersion)(IVersion.Packed &amp; 0x00FF_FFFF)</code>
    /// </summary>
    /// <remarks>
    ///   <see cref="SemVersion"/> implements <see cref="IVersion"/>
    /// </remarks>
    public SemVersion Semantic { get; }

    /// <summary>
    ///   Deconstructs the version number into the requested parts.
    /// </summary>
    /// <param name="maj"></param>
    /// <param name="min"></param>
    /// <param name="pat"></param>
    public void Deconstruct(out int maj, out int min, out int pat);

    /// <inheritdoc cref="Deconstruct(out int, out int, out int)" path="/summary"/>
    /// <typeparam name="T">int, <see cref="Types.Kind"/></typeparam>
    /// <param name="kind"></param>
    /// <param name="sem"></param>
    /// <remarks>
    /// </remarks>
    public void Deconstruct<T>(out T kind, out SemVersion sem)
        where T : unmanaged;

    /// <inheritdoc cref="Deconstruct(out int, out int, out int)" path="/summary"/>
    /// <typeparam name="T">int, <see cref="Types.Kind"/></typeparam>
    /// <param name="kind"></param>
    /// <param name="maj"></param>
    /// <param name="min"></param>
    /// <param name="pat"></param>
    public void Deconstruct<T>(out T kind, out int maj, out int min, out int pat)
        where T : unmanaged;

    public bool IsValid();
}
