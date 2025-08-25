using System;

namespace CircuitSimBackend
{
    internal class Wire : Entity
    {
        internal Wire(string name, GateType type) : base(name, type) { }

        internal Wire(string name) : this(name, GateType.WIRE) { }

        internal override string PrintClass() => "Wire";

        internal void SetState(int state) => this.State = state;

        internal void AddInput(Gate gate)
        {
            if (FanIn is not null) FanIn.Add(gate);
            else FanIn = new DataWrapper<Entity>(gate);
        }

        internal void AddOutput(Gate gate)
        {
            if (FanOut is not null) FanOut.Add(gate);
            else FanOut = new DataWrapper<Entity>(gate);
        }

        /// <summary>
        /// Creates buffer gates between each fan-in entity and each fan-out entity.
        /// Returns [firstBuffer, lastBuffer].
        /// </summary>
        internal Gate?[] CreateBuffers()
        {
            var gatePtr = FanIn;
            DataWrapper<Entity>? outPtr;

            Gate? buffer = null;
            Gate? prevBuffer = null;
            Gate? firstBuffer = null;
            Gate? lastBuffer = null;
            int count = 0;

            if (this.Type != GateType.OUTPUT && this.Type != GateType.INPUT)
            {
                // For each input
                while (gatePtr is not null)
                {
                    outPtr = FanOut;

                    // For each output
                    while (outPtr is not null /* Java had commented-out type checks here */)
                    {
                        // Create buffer name "BUF<inName><outName>"
                        var inEntity  = gatePtr.data!;
                        var outEntity = outPtr.data!;
                        var bufName = $"BUF{inEntity.Name}{outEntity.Name}";
                        buffer = new Gate(bufName, GateType.BUF);

                        // Left-side connection handling + remove this wire from inEntity's fanOut
                        if (FanOut is not null)
                        {
                            if (inEntity.FanOut is null) inEntity.FanOut = new DataWrapper<Entity>(buffer);
                            else inEntity.FanOut.Add(buffer);

                            buffer.AddFanIn(inEntity);
                            inEntity.FanOut = inEntity.DeleteOutput(this); // L
                        }

                        // Right-side connection handling + remove this wire from outEntity's fanIn
                        if (FanIn is not null)
                        {
                            if (outEntity.FanIn is null) outEntity.FanIn = new DataWrapper<Entity>(buffer);
                            else outEntity.FanIn.Add(buffer);

                            buffer.AddFanOut(outEntity);
                            outEntity.FanIn = outEntity.DeleteInput(this); // R
                        }

                        // Buffer linking
                        if (count == 0) firstBuffer = buffer;
                        else            prevBuffer!.NextGate = buffer;

                        prevBuffer = buffer;
                        outPtr = outPtr.next;
                        count++;
                    }

                    gatePtr = gatePtr.next;
                }

                lastBuffer = buffer;
            }

            return new Gate?[] { firstBuffer, lastBuffer };
        }
    }
}
