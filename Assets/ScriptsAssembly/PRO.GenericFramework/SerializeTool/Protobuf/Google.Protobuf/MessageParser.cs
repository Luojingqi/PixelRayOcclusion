﻿#region Copyright notice and license
// Protocol Buffers - Google's data interchange format
// Copyright 2015 Google Inc.  All rights reserved.
//
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file or at
// https://developers.google.com/open-source/licenses/bsd
#endregion

using PRO.Proto;
using System;
using System.Buffers;
using System.IO;
using System.Security;

namespace Google.Protobuf
{
    /// <summary>
    /// A general message parser, typically used by reflection-based code as all the methods
    /// return simple <see cref="IMessage"/>.
    /// </summary>
    public class MessageParser
    {
        private readonly Func<IMessage> factory;
        private protected bool DiscardUnknownFields { get; }

        internal ExtensionRegistry Extensions { get; }

        internal MessageParser(Func<IMessage> factory, bool discardUnknownFields, ExtensionRegistry extensions)
        {
            this.factory = factory;
            DiscardUnknownFields = discardUnknownFields;
            Extensions = extensions;
        }

        /// <summary>
        /// Creates a template instance ready for population.
        /// </summary>
        /// <returns>An empty message.</returns>
        internal IMessage CreateTemplate()
        {
            return factory();
        }

        /// <summary>
        /// Parses a message from a byte array.
        /// </summary>
        /// <param name="data">The byte array containing the message. Must not be null.</param>
        /// <returns>The newly parsed message.</returns>
        public IMessage ParseFrom(byte[] data)
        {
            IMessage message = factory();
            message.MergeFrom(data, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from a byte array slice.
        /// </summary>
        /// <param name="data">The byte array containing the message. Must not be null.</param>
        /// <param name="offset">The offset of the slice to parse.</param>
        /// <param name="length">The length of the slice to parse.</param>
        /// <returns>The newly parsed message.</returns>
        public IMessage ParseFrom(byte[] data, int offset, int length)
        {
            IMessage message = factory();
            message.MergeFrom(data, offset, length, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given byte string.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        public IMessage ParseFrom(ByteString data)
        {
            IMessage message = factory();
            message.MergeFrom(data, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given stream.
        /// </summary>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public IMessage ParseFrom(Stream input)
        {
            IMessage message = factory();
            message.MergeFrom(input, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given sequence.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        [SecuritySafeCritical]
        public IMessage ParseFrom(ReadOnlySequence<byte> data)
        {
            IMessage message = factory();
            message.MergeFrom(data, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given span.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        [SecuritySafeCritical]
        public IMessage ParseFrom(ReadOnlySpan<byte> data)
        {
            IMessage message = factory();
            message.MergeFrom(data, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a length-delimited message from the given stream.
        /// </summary>
        /// <remarks>
        /// The stream is expected to contain a length and then the data. Only the amount of data
        /// specified by the length will be consumed.
        /// </remarks>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public IMessage ParseDelimitedFrom(Stream input)
        {
            IMessage message = factory();
            message.MergeDelimitedFrom(input, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given coded input stream.
        /// </summary>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public IMessage ParseFrom(CodedInputStream input)
        {
            IMessage message = factory();
            MergeFrom(message, input);
            return message;
        }

        /// <summary>
        /// Parses a message from the given JSON.
        /// </summary>
        /// <remarks>This method always uses the default JSON parser; it is not affected by <see cref="WithDiscardUnknownFields(bool)"/>.
        /// To ignore unknown fields when parsing JSON, create a <see cref="JsonParser"/> using a <see cref="JsonParser.Settings"/>
        /// with <see cref="JsonParser.Settings.IgnoreUnknownFields"/> set to true and call <see cref="JsonParser.Parse{T}(string)"/> directly.
        /// </remarks>
        /// <param name="json">The JSON to parse.</param>
        /// <returns>The parsed message.</returns>
        /// <exception cref="InvalidJsonException">The JSON does not comply with RFC 7159</exception>
        /// <exception cref="InvalidProtocolBufferException">The JSON does not represent a Protocol Buffers message correctly</exception>
        public IMessage ParseJson(string json)
        {
            IMessage message = factory();
            JsonParser.Default.Merge(message, json);
            return message;
        }

        // TODO: When we're using a C# 7.1 compiler, make this private protected.
        internal void MergeFrom(IMessage message, CodedInputStream codedInput)
        {
            bool originalDiscard = codedInput.DiscardUnknownFields;
            ExtensionRegistry originalRegistry = codedInput.ExtensionRegistry;
            try
            {
                codedInput.DiscardUnknownFields = DiscardUnknownFields;
                codedInput.ExtensionRegistry = Extensions;
                message.MergeFrom(codedInput);
            }
            finally
            {
                codedInput.DiscardUnknownFields = originalDiscard;
                codedInput.ExtensionRegistry = originalRegistry;
            }
        }

        /// <summary>
        /// Creates a new message parser which optionally discards unknown fields when parsing.
        /// </summary>
        /// <remarks>Note that this does not affect the behavior of <see cref="ParseJson(string)"/>
        /// at all. To ignore unknown fields when parsing JSON, create a <see cref="JsonParser"/> using a <see cref="JsonParser.Settings"/>
        /// with <see cref="JsonParser.Settings.IgnoreUnknownFields"/> set to true and call <see cref="JsonParser.Parse{T}(string)"/> directly.</remarks>
        /// <param name="discardUnknownFields">Whether or not to discard unknown fields when parsing.</param>
        /// <returns>A newly configured message parser.</returns>
        public MessageParser WithDiscardUnknownFields(bool discardUnknownFields) =>
            new MessageParser(factory, discardUnknownFields, Extensions);

        /// <summary>
        /// Creates a new message parser which registers extensions from the specified registry upon creating the message instance
        /// </summary>
        /// <param name="registry">The extensions to register</param>
        /// <returns>A newly configured message parser.</returns>
        public MessageParser WithExtensionRegistry(ExtensionRegistry registry) =>
            new MessageParser(factory, DiscardUnknownFields, registry);
    }

    /// <summary>
    /// A parser for a specific message type.
    /// </summary>
    /// <remarks>
    /// <p>
    /// This delegates most behavior to the
    /// <see cref="IMessage.MergeFrom"/> implementation within the original type, but
    /// provides convenient overloads to parse from a variety of sources.
    /// </p>
    /// <p>
    /// Most applications will never need to create their own instances of this type;
    /// instead, use the static <c>Parser</c> property of a generated message type to obtain a
    /// parser for that type.
    /// </p>
    /// </remarks>
    /// <typeparam name="T">The type of message to be parsed.</typeparam>
    public sealed class MessageParser<T> : MessageParser where T : IMessage<T>
    {
        // Implementation note: all the methods here *could* just delegate up to the base class and cast the result.
        // The current implementation avoids a virtual method call and a cast, which *may* be significant in some cases.
        // Benchmarking work is required to measure the significance - but it's only a few lines of code in any case.
        // The API wouldn't change anyway - just the implementation - so this work can be deferred.
        private readonly Func<T> factory;

        /// <summary>
        /// Creates a new parser.
        /// </summary>
        /// <remarks>
        /// The factory method is effectively an optimization over using a generic constraint
        /// to require a parameterless constructor: delegates are significantly faster to execute.
        /// </remarks>
        /// <param name="factory">Function to invoke when a new, empty message is required.</param>
        public MessageParser(Func<T> factory) : this(factory, false, null)
        {
        }

        internal MessageParser(Func<T> factory, bool discardUnknownFields, ExtensionRegistry extensions) : base(() => factory(), discardUnknownFields, extensions)
        {
            //this.factory = factory;
            var pool = new ProtoPool.Pool<T>();
            lock (ProtoPool.poolDic)
                ProtoPool.poolDic.Add(typeof(T), pool);
            pool.createObjectAction = factory;
            this.factory = () => pool.TakeOut();

        }

        /// <summary>
        /// Creates a template instance ready for population.
        /// </summary>
        /// <returns>An empty message.</returns>
        internal new T CreateTemplate()
        {
            return factory();
        }

        /// <summary>
        /// Parses a message from a byte array.
        /// </summary>
        /// <param name="data">The byte array containing the message. Must not be null.</param>
        /// <returns>The newly parsed message.</returns>
        public new T ParseFrom(byte[] data)
        {
            T message = factory();
            message.MergeFrom(data, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from a byte array slice.
        /// </summary>
        /// <param name="data">The byte array containing the message. Must not be null.</param>
        /// <param name="offset">The offset of the slice to parse.</param>
        /// <param name="length">The length of the slice to parse.</param>
        /// <returns>The newly parsed message.</returns>
        public new T ParseFrom(byte[] data, int offset, int length)
        {
            T message = factory();
            message.MergeFrom(data, offset, length, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given byte string.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        public new T ParseFrom(ByteString data)
        {
            T message = factory();
            message.MergeFrom(data, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given stream.
        /// </summary>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public new T ParseFrom(Stream input)
        {
            T message = factory();
            message.MergeFrom(input, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given sequence.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        [SecuritySafeCritical]
        public new T ParseFrom(ReadOnlySequence<byte> data)
        {
            T message = factory();
            message.MergeFrom(data, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given span.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>The parsed message.</returns>
        [SecuritySafeCritical]
        public new T ParseFrom(ReadOnlySpan<byte> data)
        {
            T message = factory();
            message.MergeFrom(data, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a length-delimited message from the given stream.
        /// </summary>
        /// <remarks>
        /// The stream is expected to contain a length and then the data. Only the amount of data
        /// specified by the length will be consumed.
        /// </remarks>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public new T ParseDelimitedFrom(Stream input)
        {
            T message = factory();
            message.MergeDelimitedFrom(input, DiscardUnknownFields, Extensions);
            return message;
        }

        /// <summary>
        /// Parses a message from the given coded input stream.
        /// </summary>
        /// <param name="input">The stream to parse.</param>
        /// <returns>The parsed message.</returns>
        public new T ParseFrom(CodedInputStream input)
        {
            T message = factory();
            MergeFrom(message, input);
            return message;
        }

        /// <summary>
        /// Parses a message from the given JSON.
        /// </summary>
        /// <param name="json">The JSON to parse.</param>
        /// <returns>The parsed message.</returns>
        /// <exception cref="InvalidJsonException">The JSON does not comply with RFC 7159</exception>
        /// <exception cref="InvalidProtocolBufferException">The JSON does not represent a Protocol Buffers message correctly</exception>
        public new T ParseJson(string json)
        {
            T message = factory();
            JsonParser.Default.Merge(message, json);
            return message;
        }

        /// <summary>
        /// Creates a new message parser which optionally discards unknown fields when parsing.
        /// </summary>
        /// <param name="discardUnknownFields">Whether or not to discard unknown fields when parsing.</param>
        /// <returns>A newly configured message parser.</returns>
        public new MessageParser<T> WithDiscardUnknownFields(bool discardUnknownFields) =>
            new MessageParser<T>(factory, discardUnknownFields, Extensions);

        /// <summary>
        /// Creates a new message parser which registers extensions from the specified registry upon creating the message instance
        /// </summary>
        /// <param name="registry">The extensions to register</param>
        /// <returns>A newly configured message parser.</returns>
        public new MessageParser<T> WithExtensionRegistry(ExtensionRegistry registry) =>
            new MessageParser<T>(factory, DiscardUnknownFields, registry);
    }
}
