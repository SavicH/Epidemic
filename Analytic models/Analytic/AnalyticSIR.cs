﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompartmentModels.Analytic
{
    abstract class AnalyticSIR : AnalyticSIS
    {
        protected override void FillState(ref State state, double[,] solution, int row)
        {
            base.FillState(ref state, solution, row);
            state.Removed = solution[row, 3];
        }

        public AnalyticSIR(State initialState, Parameters parameters, double time, double timestep)
            : base(initialState, parameters, time, timestep)
        {
            compartmentsCount = 3;
        }
    }
}
