﻿using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Microsoft.SqlServer.Management.Smo;
using TypeMock.ArrangeActAssert;
using Xpand.ExpressApp.WorldCreator.SqlDBMapper;
using Xpand.Persistent.Base.PersistentMetaData;
using Xpand.Persistent.Base.PersistentMetaData.PersistentAttributeInfos;
using Xpand.Persistent.BaseImpl.PersistentMetaData.PersistentAttributeInfos;

namespace Xpand.Tests.Xpand.WorldCreator.DbMapper {

    public class When_a_column_is_nullable : With_Column {
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;

        Establish context = () => {

            _column.Nullable = false;
        };

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, Isolate.Fake.Instance<IPersistentMemberInfo>(), new DataTypeMapper());
        };

        It should_create_a_rule_required_attribute =
            () => _persistentAttributeInfos.OfType<PersistentRuleRequiredFieldAttribute>().FirstOrDefault().ShouldNotBeNull();
    }

    public class When_column_size_is_100 : With_Column {
        static IEnumerable<IPersistentAttributeInfo> _persistentAttributeInfos;
        Establish context = () => {
            _column.DataType.MaximumLength = 100;
            _column.DataType.SqlDataType = SqlDataType.Text;
        };

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, Isolate.Fake.Instance<IPersistentMemberInfo>(), new DataTypeMapper());
        };

        It should_create_a_size_attribute_lenght_100 =
            () => _persistentAttributeInfos.OfType<PersistentSizeAttribute>().Single().Size.ShouldEqual(100);
    }

    public class When_column_is_primary_key : With_Column {
        static IEnumerable<IPersistentAttributeInfo> _persistentAttributeInfos;
        Establish context = () => Isolate.WhenCalled(() => _column.InPrimaryKey).WillReturn(true);

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, Isolate.Fake.Instance<IPersistentMemberInfo>(), new DataTypeMapper());
        };

        It should_create_persistent_key_attribute =
            () => _persistentAttributeInfos.OfType<PersistentKeyAttribute>().FirstOrDefault().ShouldNotBeNull();
    }

    public class When_column_is_primary_key_and_autogenerated : With_Column {
        static IEnumerable<IPersistentAttributeInfo> _persistentAttributeInfos;
        Establish context = () => {
            Isolate.WhenCalled(() => _column.InPrimaryKey).WillReturn(true);
            Isolate.WhenCalled(() => _column.Identity).WillReturn(true);
        };

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, Isolate.Fake.Instance<IPersistentMemberInfo>(), new DataTypeMapper());
        };

        It should_create_an_autogenerated_persistentkey_attribute =
            () => _persistentAttributeInfos.OfType<PersistentKeyAttribute>().Single().AutoGenerated.ShouldEqual(true);
    }

    public class When_column_is_foreign : With_ForeignKey_Column {
        static PersistentAssociationAttribute _persistentAssociationAttribute;
        static IEnumerable<IPersistentAttributeInfo> _persistentAttributeInfos;

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, Isolate.Fake.Instance<IPersistentMemberInfo>(), new DataTypeMapper()).ToList();
        };

        It should_create_an_association_attribute = () => {
            _persistentAssociationAttribute = _persistentAttributeInfos.OfType<PersistentAssociationAttribute>().FirstOrDefault();
            _persistentAssociationAttribute.ShouldNotBeNull();
        };

        It should_have_as_association_the_name_of_the_foreignKey_that_this_column_belongs = () => _persistentAssociationAttribute.AssociationName.ShouldEqual(FK_Name);
    }

    public class When_column_is_primarykey_and_table_has_more_privae_keys : With_Column {
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;

        Establish context = () => {
            Isolate.WhenCalled(() => _column.InPrimaryKey).WillReturn(true);
            _table.Columns.Add(_column);
        };

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, Isolate.Fake.Instance<IPersistentMemberInfo>(), new DataTypeMapper());
        };

        It should_return_a_keyAttribute =
            () => _persistentAttributeInfos.OfType<PersistentKeyAttribute>().FirstOrDefault().ShouldNotBeNull();

        It should_return_a_persistent_attribute =
            () => _persistentAttributeInfos.OfType<PersistentPersistentAttribute>();
    }

    public class When_column_is_foreign_and_primary_key_and_table_has_combound_keys : With_ForeignKey_Column {
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;
        static IPersistentMemberInfo _persistentMemberInfo;

        Establish context = () => {
            Isolate.WhenCalled(() => _column.InPrimaryKey).WillReturn(true);
            _persistentMemberInfo = Isolate.Fake.Instance<IPersistentMemberInfo>();
            _persistentMemberInfo.Owner.CodeTemplateInfo.CodeTemplate.TemplateType = TemplateType.Struct;
        };

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, _persistentMemberInfo, new DataTypeMapper());
        };

        It should_create_an_assocation_attribute =
            () => _persistentAttributeInfos.OfType<IPersistentAssociationAttribute>().FirstOrDefault().ShouldNotBeNull();

        It should_not_create_a_key_attribute =
            () => _persistentAttributeInfos.OfType<IPersistentKeyAttribute>().FirstOrDefault().ShouldBeNull();

    }

    public class When_column_is_combound_foreign_key : With_ForeignKey_Column {
        static IPersistentPersistentAttribute _persistentPersistentAttribute;
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;

        Establish context = () => {
            var foreignKeyColumn = Isolate.Fake.Instance<ForeignKeyColumn>();
            _foreignKey.Columns.Add(foreignKeyColumn);
        };

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, Isolate.Fake.Instance<IPersistentMemberInfo>(), new DataTypeMapper());
        };

        It should_create_a_persitent_attrbiute = () => {
            _persistentPersistentAttribute =
                _persistentAttributeInfos.OfType<IPersistentPersistentAttribute>().FirstOrDefault();
            _persistentPersistentAttribute.ShouldNotBeNull();
        };

        It should_map_it_to_an_empty_string = () => _persistentPersistentAttribute.MapTo.ShouldEqual(String.Empty);
    }

    public class When_owner_is_one_to_one : With_ForeignKey_Column {
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;
        static IPersistentMemberInfo _persistentMemberInfo;

        Establish context = () => {
            _persistentMemberInfo = Isolate.Fake.Instance<IPersistentMemberInfo>();
            _persistentMemberInfo.CodeTemplateInfo.CodeTemplate.TemplateType = TemplateType.XPOneToOnePropertyMember;
        };

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, _persistentMemberInfo, new DataTypeMapper());
        };

        It should_not_create_any_attribute = () => _persistentAttributeInfos.Count.ShouldEqual(0);
    }

    public class When_column_is_mapped : With_Column {
        static IPersistentPersistentAttribute _persistentPersistentAttribute;
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_column, Isolate.Fake.Instance<IPersistentMemberInfo>(), new DataTypeMapper());
        };

        It should_create_a_persistent_attribute = () => {
            _persistentPersistentAttribute =
                _persistentAttributeInfos.OfType<IPersistentPersistentAttribute>().FirstOrDefault();
            _persistentPersistentAttribute.ShouldNotBeNull();
        };

        It should_map_it_to_column_name = () => _persistentPersistentAttribute.MapTo.ShouldEqual(_column.Name);
    }
    public class When_table_is_mapped : With_table {
        static IPersistentPersistentAttribute _persistentPersistentAttribute;
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_table, Isolate.Fake.Instance<IPersistentClassInfo>(), Isolate.Fake.Instance<IMapperInfo>());
        };

        It should_create_a_persistent_attribute = () => {
            _persistentPersistentAttribute =
                _persistentAttributeInfos.OfType<IPersistentPersistentAttribute>().FirstOrDefault();
            _persistentPersistentAttribute.ShouldNotBeNull();
        };

        It should_map_it_to_column_name = () => _persistentPersistentAttribute.MapTo.ShouldEqual(_table.Name);
    }

    public class When_table_owner_has_a_persiste_attribute : With_table {
        static IPersistentClassInfo _persistentClassInfo;
        static List<IPersistentAttributeInfo> _persistentAttributeInfos;

        Establish context = () => {
            _persistentClassInfo = Isolate.Fake.Instance<IPersistentClassInfo>();
            Isolate.WhenCalled(() => _persistentClassInfo.TypeAttributes).WillReturn(new List<IPersistentAttributeInfo> { Isolate.Fake.Instance<IPersistentPersistentAttribute>() });
        };

        Because of = () => {
            _persistentAttributeInfos = new AttributeMapper(ObjectSpace).Create(_table, _persistentClassInfo, Isolate.Fake.Instance<IMapperInfo>());
        };

        It should_not_return_any_attribute = () => _persistentAttributeInfos.OfType<IPersistentPersistentAttribute>().Count().ShouldEqual(0);
    }

}
