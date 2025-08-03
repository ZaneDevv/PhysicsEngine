using System;

namespace PhysicsEngine.Bodies
{
    internal class Body
    {
        private bool isCollideable = true;
        private bool doesPhysicsAffect = true;

        internal bool IsCollideable { get { return this.isCollideable; } set => this.isCollideable = value; }
        internal bool DoesPhysicsAffect { get { return this.doesPhysicsAffect; } set => this.doesPhysicsAffect = value; }
    }
}
