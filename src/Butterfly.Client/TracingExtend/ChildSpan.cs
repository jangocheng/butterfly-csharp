﻿using System;
using System.Threading;
using Butterfly.OpenTracing;

namespace Butterfly.Client
{
    public class ChildSpan : ISpan
    {
        private readonly ISpan _span;
        private readonly ISpan _parent;
        private readonly ITracer _tracer;
        private int _state;

        public DateTimeOffset StartTimestamp => _span.StartTimestamp;

        public DateTimeOffset FinishTimestamp => _span.FinishTimestamp;

        public string OperationName => _span.OperationName;

        public ISpanContext SpanContext => _span.SpanContext;

        public TagCollection Tags => _span.Tags;

        public LogCollection Logs => _span.Logs;

        public ChildSpan(ISpan span, ITracer tracer)
        {
            _span = span;
            _tracer = tracer;
            _parent = _tracer.GetCurrentSpan();
            _tracer.SetCurrentSpan(span);
        }

        public void Dispose()
        {
            Finish(DateTimeOffset.UtcNow);
        }

        public void Finish(DateTimeOffset finishTimestamp)
        {
            if (Interlocked.CompareExchange(ref _state, 1, 0) != 1)
            {
                _span.Finish(finishTimestamp);
                _tracer.SetCurrentSpan(_parent);
            }
        }
    }
}