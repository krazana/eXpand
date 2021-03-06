﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.Security;
using Xpand.ExpressApp.ModelDifference.DataStore.BaseObjects;
using Xpand.ExpressApp.ModelDifference.DataStore.Queries;
using Xpand.ExpressApp.ModelDifference.Security;
using Xpand.Xpo.Collections;

namespace FeatureCenter.Module.ApplicationDifferences {

    public class Updater : FCUpdater {
        private const string ModelCombine = "ModelCombine";

        public Updater(IObjectSpace objectSpace, Version currentDBVersion, Xpand.Persistent.BaseImpl.Updater updater)
            : base(objectSpace, currentDBVersion, updater) {
        }



        public override void UpdateDatabaseAfterUpdateSchema() {
            var session = ((ObjectSpace)ObjectSpace).Session;
            if (new QueryModelDifferenceObject(session).GetActiveModelDifference(ModelCombine, FeatureCenterModule.Application) == null) {
                new ModelDifferenceObject(session).InitializeMembers(ModelCombine, FeatureCenterModule.Application);
                ICustomizableRole role = Updater.EnsureRoleExists(ModelCombine, customizableRole => GetPermissions(customizableRole, Updater));
                IUserWithRoles user = Updater.EnsureUserExists(ModelCombine, ModelCombine, role);
                role.AddPermission(new ModelCombinePermission(ApplicationModelCombineModifier.Allow) { Difference = ModelCombine });
                role.Users.Add(user);
                ObjectSpace.CommitChanges();
            }
            var modelDifferenceObjects = new XpandXPCollection<ModelDifferenceObject>(session, o => o.PersistentApplication.Name == "FeatureCenter");
            foreach (var modelDifferenceObject in modelDifferenceObjects) {
                modelDifferenceObject.PersistentApplication.Name =
                    Path.GetFileNameWithoutExtension(modelDifferenceObject.PersistentApplication.ExecutableName);
            }
            ObjectSpace.CommitChanges();
        }
        protected List<IPermission> GetPermissions(ICustomizableRole customizableRole, Xpand.Persistent.BaseImpl.Updater updater) {
            var permissions = updater.GetPermissions(customizableRole);
            if (customizableRole.Name == ModelCombine)
                permissions.Add(new EditModelPermission(ModelAccessModifier.Allow));
            return permissions;
        }
    }
}
