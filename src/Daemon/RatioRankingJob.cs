using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daemon {
    [DisallowConcurrentExecution]
    class RatioRankingJob : IJob {
        public void Execute(IJobExecutionContext context) {
        }
    }
}
