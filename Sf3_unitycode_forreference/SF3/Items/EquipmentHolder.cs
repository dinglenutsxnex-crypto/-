using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SF3.Items
{
	public class EquipmentHolder : ClassHolder<Equipment>, IEquipmentHolder
	{
		public EquipmentHolderPurpose HolderType { get; private set; }

		private EquipmentHolder(EquipmentHolderPurpose purpose)
		{
			Init(purpose);
			Fill(purpose);
		}

		public static EquipmentHolder Create()
		{
			return new EquipmentHolder(EquipmentHolderPurpose.Empty);
		}

		public static EquipmentHolder Create(EquipmentHolderPurpose purpose)
		{
			return new EquipmentHolder(purpose);
		}

		private void Init(EquipmentHolderPurpose purpose)
		{
			HolderType = purpose;
			foreach (EquipmentType value in Enum.GetValues(typeof(EquipmentType)))
			{
				Holder.Add(GetNameByType(value), new List<Equipment>());
			}
		}

		private void Fill(EquipmentHolderPurpose purpose)
		{
			if (purpose != EquipmentHolderPurpose.FilledFromJs)
			{
				return;
			}
			foreach (Equipment value in JS.Instance.Equipment.Values)
			{
				Holder[GetNameByType(value.GetEquipmentType())].Add((Equipment)value.Clone());
			}
		}

		private string GetNameByType(EquipmentType type)
		{
			return ComplianceUtils.GetEquipmentNameByType(type);
		}

		private IEnumerable<Equipment> GetEquipmentAll()
		{
			return from EquipmentType name in Enum.GetValues(typeof(EquipmentType))
				from equipment in Holder[GetNameByType(name)]
				select equipment;
		}

		public Equipment GetEquipmentById(int id)
		{
			return GetEquipmentAll().SingleOrDefault((Equipment equipment) => equipment.id == id);
		}

		public Equipment GetEquipmentByIdClone(int id)
		{
			Equipment equipmentById = GetEquipmentById(id);
			return (equipmentById != null) ? ((Equipment)equipmentById.Clone()) : null;
		}

		public void AddEquipment(Equipment equipment)
		{
			if (equipment != null)
			{
				Holder[GetNameByType(equipment.GetEquipmentType())].Add(equipment);
			}
		}

		public void AddEquipment(List<Equipment> equipmentLoaded)
		{
			foreach (Equipment item in equipmentLoaded)
			{
				AddEquipment(item);
			}
		}

		public List<Equipment> GetEquipmentByTypeAll(EquipmentType type)
		{
			return Holder[GetNameByType(type)];
		}

		public List<Equipment> GetEquipmentByTypeAllSafe(EquipmentType type)
		{
			return (from equipment in GetEquipmentByTypeAll(type)
				select (Equipment)equipment.Clone()).ToList();
		}

		public List<Equipment> GetEquipmentDefaultByTypeAll(EquipmentType type)
		{
			return (from equipment in GetEquipmentByTypeAll(type)
				where equipment.IsDefault()
				select equipment).ToList();
		}

		public List<Equipment> GetEquipmentDefaultByTypeAllClone(EquipmentType type)
		{
			return (from equipment in GetEquipmentDefaultByTypeAll(type)
				select (Equipment)equipment.Clone()).ToList();
		}

		private void LogError(string msg)
		{
			StringWrapper stringWrapper = StringWrapper.Create(StringWrapperPurpose.Error);
			stringWrapper.Head("EquipmentHolder, Type : <" + HolderType.ToString() + ">");
			stringWrapper.Add(msg);
			Debug.LogError(stringWrapper.ToString());
		}
	}
}
