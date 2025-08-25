using System;

namespace CircuitSimBackend
{
    public enum GateType
    {
        INPUT,
        OUTPUT,
        WIRE,
        AND,
        NAND,
        OR,
        NOR,
        NOT,
        DFF,
        BUF
    }

    public static class GateTypeHelper
    {
        /// <summary>Read a GateType as a string.</summary>
        public static string ReadType(GateType type)
        {
            switch (type)
            {
                case GateType.INPUT:  return "INPUT";
                case GateType.OUTPUT: return "OUTPUT";
                case GateType.WIRE:   return "WIRE";
                case GateType.AND:    return "AND";
                case GateType.NAND:   return "NAND";
                case GateType.OR:     return "OR";
                case GateType.NOR:    return "NOR";
                case GateType.NOT:    return "NOT";
                case GateType.DFF:    return "DFF";
                case GateType.BUF:    return "BUF";
                default:
                    Console.Error.WriteLine("Invalid gate type!");
                    return string.Empty;
            }
        }

        /// <summary>Read a GateType as GateType.</summary>
        public static GateType? ReadType(string type)
        {
            switch (type.ToLower())
            {
                case "input":  return GateType.INPUT;
                case "output": return GateType.OUTPUT;
                case "wire":   return GateType.WIRE;
                case "and":    return GateType.AND;
                case "nand":   return GateType.NAND;
                case "or":     return GateType.OR;
                case "nor":    return GateType.NOR;
                case "not":    return GateType.NOT;
                case "dff":    return GateType.DFF;
                case "buf":    return GateType.BUF;
                default:
                    Console.Error.WriteLine("Tried to parse invalid string!");
                    return null; // nullable return matches Java’s null
            }
        }
    }
}
