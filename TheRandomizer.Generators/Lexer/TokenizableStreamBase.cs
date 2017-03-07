using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheRandomizer.Generators.Lexer
{
    internal class TokenizableStreamBase<T> where T : class
    {

        public TokenizableStreamBase(Func<List<T>> extractor)
        {
            Index = 0;
            Items = extractor();
            SnapshotIndexes = new Stack<int>();
        }

        protected int Index { get; set; }
        private List<T> Items { get; set; }
        private Stack<int> SnapshotIndexes { get; set; }

        public virtual T Current
        {
            get
            {
                if (EOF(0))
                {
                    return null;
                }
                return Items[Index];
            }
        }

        public void Consume()
        {
            Index++;
        }

        private Boolean BOF(int lookbehind)
        {
            if (Index + lookbehind < 0)
            {
                return true;
            }
            return false;
        }

        private Boolean EOF(int lookahead)
        {
            if (Index + lookahead >= Items.Count)
            {
                return true;
            }
            return false;
        }

        public Boolean End()
        {
            return EOF(0);
        }

        public virtual T Peek(int lookahead)
        {
            if (EOF(lookahead) || BOF(lookahead))
            {
                return null;
            }
            return Items[Index + lookahead];
        }

        public void TakeSnapshot()
        {
            SnapshotIndexes.Push(Index);
        }

        public void RollbackSnapshot()
        {
            Index = SnapshotIndexes.Pop();
        }

        public void CommitSnapshot()
        {
            SnapshotIndexes.Pop();
        }
    }
}
