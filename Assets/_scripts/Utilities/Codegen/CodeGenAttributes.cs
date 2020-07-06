using System;

namespace Codegen {
    public class Trigger : Attribute
    {
        public Trigger() { }
        public Trigger(Type componentType)
        {
            this.type = componentType;
        }
        public Type type;
    }
    public class OnTriggerEnter2D : Trigger { }
    public class OnTriggerExit2D : Trigger { }
    public class OnTriggerStay2D : Trigger { }
}
