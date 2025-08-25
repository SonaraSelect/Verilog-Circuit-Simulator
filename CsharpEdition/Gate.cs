using System.IO;

namespace CircuitSimBackend
{
    internal class Gate(string Name, GateType Type) : Entity(Name, Type)
    {

        internal Gate? NextGate { get; set; }

        internal override string PrintClass() => "Gate";

        // Format is OUTPUT, inputs<-->
        internal void AddFanIn(Entity entity)
        {
            if (FanIn != null)
            {
                FanIn.Add(entity);
            }
            else
            {
                FanIn = new DataWrapper<Entity>(entity);
            }
        }

        internal void AddFanOut(Entity entity)
        {
            if (FanOut != null)
            {
                FanOut.Add(entity);
            }
            else
            {
                FanOut = new DataWrapper<Entity>(entity);
            }
        }

        internal void PrintDetails(TextWriter writer)
        {
            // Handle null safety for FanIn and FanOut
            int fanInCount   = FanIn?.Count() ?? 0;
            string fanInWires  = FanIn?.ToString() ?? "N/A";
            int fanOutCount  = FanOut?.Count() ?? 0;
            string fanOutWires = FanOut?.ToString() ?? "N/A";
            string outputWire  = FanOut?.data?.ToString() ?? "N/A";

            // Composite formatting: {index,alignment}
            writer.WriteLine(
                "{0,-10} {1,-10} {2,-10} {3,-10} {4,-20} {5,-10} {6,-20} {7,-10}",
                Type.ToString(),   // 0: gate type
                outputWire,        // 1: output wire
                Level,             // 2: gate level
                fanInCount,        // 3: fan-in count
                fanInWires,        // 4: fan-in wires
                fanOutCount,       // 5: fan-out count
                fanOutWires,       // 6: fan-out wires
                Name               // 7: gate name
            );
        }

    }
}