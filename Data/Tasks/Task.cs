namespace FASTASelector.Data
{
    partial class Controller
    {
        private abstract class Task
        {

            public abstract bool Initialize( Controller controller );


            public abstract void Redo( Controller controller );


            public abstract void Undo( Controller controller );

        }
    }
}
