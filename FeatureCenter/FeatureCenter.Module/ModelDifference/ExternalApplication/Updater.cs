﻿using System;
using DevExpress.ExpressApp.Updating;
using DevExpress.Xpo;
using eXpand.ExpressApp.ModelDifference.Core;
using eXpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using eXpand.ExpressApp.ModelDifference.DataStore.Queries;

namespace FeatureCenter.Module.ModelDifference.ExternalApplication
{
    public class Updater:ModuleUpdater
    {
        public Updater(Session session, Version currentDBVersion) : base(session, currentDBVersion) {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
//            return;

            var uniqueName = new ExternalApplicationModelStore().Name;
            if (new QueryModelDifferenceObject(Session).GetActiveModelDifference(uniqueName, "ExternalApplication") == null){
                var modelDifferenceObject = new ModelDifferenceObject(Session).InitializeMembers("ExternalApplication", "ExternalApplication", uniqueName);
                modelDifferenceObject.PersistentApplication.ExecutableName = "ExternalApplication.Win.exe";
                modelDifferenceObject.Model = new ModelApplicationBuilder(modelDifferenceObject.PersistentApplication.ExecutableName).GetLayer(typeof(ExternalApplicationModelStore));
                modelDifferenceObject.Save();
                
            }
        }
    }
}
