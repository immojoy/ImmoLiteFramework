
namespace Immojoy.LiteFramework.Runtime
{
    /// <summary>
    /// Procedure base class.</br>
    /// Procedure is a state in the game runtime lifecycle, such as launch procedure, main menu procedure, game procedure, etc.
    /// </summary>
    public abstract class ImmoProcedureBase : ImmoFsmState<ImmoProcedureManager>
    {
        /// <summary>
        /// Called when procedure is initialized.
        /// </summary>
        /// <param name="procedureOwner">Procedure owner.</param>
        protected internal override void OnInit(IImmoFsm<ImmoProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
        }


        /// <summary>
        /// Called when entering procedure.
        /// </summary>
        /// <param name="procedureOwner">Procedure owner.</param>
        protected internal override void OnEnter(IImmoFsm<ImmoProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }


        /// <summary>
        /// Called when procedure is polled.
        /// </summary>
        /// <param name="procedureOwner">Procedure owner.</param>
        /// <param name="elapseSeconds">Logical elapsed time in seconds.</param>
        /// <param name="realElapseSeconds">Real elapsed time in seconds.</param>
        protected internal override void OnUpdate(IImmoFsm<ImmoProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        }


        /// <summary>
        /// Called when leaving procedure.
        /// </summary>
        /// <param name="procedureOwner">Procedure owner.</param>
        /// <param name="isShutdown">Whether triggered by shutting down procedure manager.</param>
        protected internal override void OnLeave(IImmoFsm<ImmoProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }


        /// <summary>
        /// Called when procedure is destroyed.
        /// </summary>
        /// <param name="procedureOwner">Procedure owner.</param>
        protected internal override void OnDestroy(IImmoFsm<ImmoProcedureManager> procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
    }
}
