using System;
using System.Collections.Generic;

namespace CircuitSimBackend
{
    /// <summary>A class used to model the shared behaviors between gate and wire entities. The actual linking of entities (adding to their fan in and fan out) will be handled externally to increase efficiency and reduce code redundancy</summary>
    public class Entity
    {
        internal String Name { get; }
        internal GateType Type { get; }
        internal DataWrapper<Entity>? FanIn { get; set; }
        internal DataWrapper<Entity>? FanOut { get; set; }
        internal int State { get; set; }
        internal int Level { get; set; }

        public Entity(String name, GateType type)
        {
            this.Name = name;
            this.Type = type;
            this.FanIn = null;
            this.FanOut = null;
            this.Level = -1;
            this.State = 4;
        }

        /// <summary>Print whether or not the entity instance is a gate, wire, or raw entity.</summary>
        /// <remarks>In actual use of the program, there should be no "Entities." Only wires and gates.</remarks>
        public String printClass()
        {
            return "Entity";
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal DataWrapper<Entity>? DeleteInput(Entity data)
        {
            if (FanIn == null) return null;
            return deleteFromList(data, FanIn);
        }

        internal DataWrapper<Entity>? DeleteOutput(Entity data)
        {
            if (FanOut == null) return null;
            return deleteFromList(data, FanOut);
        }

        /// <summary>Delete a specified entity from fan in or fan out</summary>
        private DataWrapper<Entity>? deleteFromList(Entity data, DataWrapper<Entity> list)
        {
            // Check to see if list is single entry
            if (list.next == null)
            {
                // Delete single entry
                if (list.data == data)
                {
                    return null;
                    // Data not in single entry
                }
                else
                {
                    return list;
                }
                // Multiple entries in list
            }
            else
            {
                DataWrapper<Entity> first = list;
                DataWrapper<Entity> ptr = first;
                // Check if item in first entry
                if (list.data == data)
                {
                    return list.next;
                }
                // iterate to data entry before target (or to END of list)
                while (ptr.next != null && ptr.next.data != data)
                {
                    ptr = ptr.next;
                }
                // Check to see if next entry is the target (otherwise next entry does not
                // exist)
                if (ptr.next != null && ptr.next.data == data)
                {
                    if (ptr.next.next != null)
                    {
                        ptr.next = ptr.next.next;
                    }
                    else
                    {
                        ptr.next = null;
                    }
                }
                return first;
            }
        }

        internal void calculateLevels(int newLevel, Dictionary<int, Dictionary<String, Entity>> sched)
        {
            // If we reach a D flip flop, stop and do not calibrate
            if (this.Type == GateType.DFF)
                return;
            // If entity's max level is lower than its new level, calibrate and traverse outputs
            if (this.Level < newLevel)
            {
                int oldLevel = this.Level;
                this.Level = newLevel;
                recordLevel(oldLevel, newLevel, sched);
                DataWrapper<Entity>? ptr = this.FanOut;
                while (ptr != null)
                {
                    if (ptr.data != null)
                        ptr.data.calculateLevels(newLevel + 1, sched);
                    ptr = ptr.next;
                }
            }
        }

        /// <summary>Helper method for calculateLevels that checks to see if the entity is already logged into the sched data set, and updates the value accordingly</summary>
        internal void recordLevel(int oldLevel, int newLevel, Dictionary<int, Dictionary<String, Entity>> sched)
        {
            // If the entity was previously logged into sched, remove it
            if (sched.TryGetValue(oldLevel, out var oldMap) && oldMap.ContainsKey(this.Name))
            {
                oldMap.Remove(this.Name);
            }

            // If sched does not have a map for current level, create it
            if (!sched.TryGetValue(newLevel, out var newMap))
            {
                newMap = new Dictionary<string, Entity>(capacity: 100);
                sched[newLevel] = newMap;
            }

            // Add entity to new spot in sched
            newMap[this.Name] = this;
        }

        internal void calculateState()
        {
            if (this.Type == GateType.INPUT || this.FanIn == null) return;
            var input = FanIn.data;
            if (input == null) return;

            switch (this.Type)
            {
                case GateType.DFF:
                    State = input.State;
                    break;
                case GateType.BUF:
                    State = input.State;
                    break;
                case GateType.OUTPUT:
                    State = input.State;
                    break;
                case GateType.AND:
                    runAND();
                    break;
                case GateType.NAND:
                    runNAND();
                    break;
                case GateType.OR:
                    runOR();
                    break;
                case GateType.NOR:
                    runNOR();
                    break;
                case GateType.NOT:
                    this.State = calcNOT(input.State);
                    break;
                default:
                    break;
            }
        }

        private void runAND()
        {
            int lastCalc; // holds 'sum' of last 2 inputs
            if (FanIn == null) return;
            DataWrapper<Entity>? ptr = FanIn;
            if (ptr != null && ptr.data != null)
            {
                lastCalc = ptr.data.State;
                ptr = ptr.next;
                while (ptr != null && ptr.data != null)
                {
                    lastCalc = calcAND(lastCalc, ptr.data.State);
                    ptr = ptr.next;
                }
                this.State = lastCalc;
            }
        }

        void runNAND()
        {
            int lastCalc; // holds 'sum' of last 2 inputs
            if (FanIn != null && FanIn.data != null)
            {
                DataWrapper<Entity>? ptr = FanIn;
                lastCalc = ptr.data.State;
                ptr = ptr.next;
                while (ptr != null && ptr.data != null)
                {
                    lastCalc = calcAND(lastCalc, ptr.data.State);
                    ptr = ptr.next;
                }
                this.State = calcNOT(lastCalc);
            }
        }

        void runOR()
        {
            if (FanIn != null && FanIn.data != null)
            {
                int lastCalc; // holds 'sum' of last 2 inputs
                DataWrapper<Entity>? ptr = FanIn;
                lastCalc = ptr.data.State;
                ptr = ptr.next;
                while (ptr != null && ptr.data != null)
                {
                    lastCalc = calcOR(lastCalc, ptr.data.State);
                    ptr = ptr.next;
                }
                this.State = lastCalc;
            }
        }

        void runNOR()
        {
            if (FanIn != null && FanIn.data != null)
            {
                int lastCalc; // holds 'sum' of last 2 inputs
                DataWrapper<Entity>? ptr = FanIn;
                lastCalc = ptr.data.State;
                ptr = ptr.next;
                while (ptr != null && ptr.data != null)
                {
                    lastCalc = calcOR(lastCalc, ptr.data.State);
                    ptr = ptr.next;
                }
                this.State = calcNOT(lastCalc);
            }
        }

        int calcAND(int x, int y)
        {
            if (x == 0 || y == 0)
                return 0;
            if (x == 1 && y == 1)
                return 1;
            return 4;
        }

        int calcOR(int x, int y)
        {
            if (x == 1 || y == 1)
                return 1;
            if (x == 0 && y == 0)
                return 0;
            return 4;
        }
        
        int calcNOT(int x) {
            if (x == 1)
                return 0;
            if (x == 0)
                return 1;
            return 4;
        }

    }
}