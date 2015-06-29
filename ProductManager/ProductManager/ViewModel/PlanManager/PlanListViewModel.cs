using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using ProductManager.ViewData.Entity;
using System.Collections.Generic;
using ProductManager.Controls;

namespace ProductManager.ViewModel.PlanManager
{
    public class PlanListViewModel : NotifyPropertyChanged
    {
        public ObservableCollection<PlanEntity> PlanList { get; set; }

        public ObservableCollection<PlanEntity> FilterPlanList { get; set; }

        public Dictionary<int, string> DepartmentIdNameDictionary { get; set; }

        public ObservableCollection<PlanEntity> DeletedPlanList { get; set; }

        public int ColumnModelIndex = -1;

        public string Title { get; set; }

        public string EditingOriginalName
        {
            get
            {
                string returnValue = "正在编辑序号：";
                if (null != uneditedPlanEntity)
                {
                    returnValue += uneditedPlanEntity.SequenceId;
                }
                return returnValue;
            }
        }

        private PlanEntity uneditedPlanEntity = null;

        private PlanEntity editingPlanEntity = new PlanEntity();
        public PlanEntity EditingPlanEntity
        {
            get
            {
                return editingPlanEntity;
            }
            set
            {
                if (editingPlanEntity != value)
                {
                    editingPlanEntity = value;
                    editingPlanEntity.IsScoreSetable = (editingPlanEntity.IsScoreSetable && isEditing);
                    UpdateChanged("EditingPlanEntity");
                    (OnAddPlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnEditPlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnDeletePlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnInsertPlan as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private PlanEntity selectPlanEntity = null;
        public PlanEntity SelectPlanEntity
        {
            get
            {
                return selectPlanEntity;
            }
            set
            {
                if (selectPlanEntity != value)
                {
                    selectPlanEntity = value;                    
                    UpdateChanged("SelectPlanEntity");
//                     (OnAddPlan as DelegateCommand).RaiseCanExecuteChanged();
//                     (OnEditPlan as DelegateCommand).RaiseCanExecuteChanged();
//                     (OnDeletePlan as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    UpdateChanged("IsBusy");
                }
            }
        }

        private bool isChanged = true;
        public bool IsChanged
        {
            get { return isChanged; }
            set
            {
                if (isChanged != value)
                {
                    isChanged = value;
                    UpdateChanged("IsChanged");
                }
            }
        }

        private bool isReadOnly = false;
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                if (isReadOnly != value)
                {
                    isReadOnly = value;
                    UpdateChanged("IsReadOnly");
                }
            }
        }

        private bool isEditing = false;
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                if (isEditing != value)
                {
                    isEditing = value;
                    UpdateChanged("IsEditing");
                    UpdateChanged("IsNotEditing");

                    isChanged = (isChanged | isEditing);
                    UpdateChanged("IsChanged");

                    EditingPlanEntity.IsScoreSetable = (EditingPlanEntity.IsScoreSetable && isEditing);
                    (OnAddPlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnEditPlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnDeletePlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnInsertPlan as DelegateCommand).RaiseCanExecuteChanged();
                    (OnOK as DelegateCommand).RaiseCanExecuteChanged();
                    (OnCancel as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsNotEditing
        {
            get { return !isEditing; }
        }

        private bool CanAdd(object aObject)
        {
            return !isEditing;
        }

        private bool CanEdit(object aObject)
        {
            return null != selectPlanEntity && !isEditing;
        }

        private bool CanOKCancel(object aObject)
        {
            return isEditing;
        }

        public PlanListViewModel(string aTitle, ObservableCollection<PlanEntity> aPlanList,
            int aColumnModelIndex, Dictionary<int, string> aDepartmentIdNameDictionary)
        {
            this.Title = aTitle;
            this.PlanList = aPlanList;
            this.DepartmentIdNameDictionary = aDepartmentIdNameDictionary;
            this.ColumnModelIndex = aColumnModelIndex;

            this.FilterPlanList = new ObservableCollection<PlanEntity>();
            this.DeletedPlanList = new ObservableCollection<PlanEntity>();
//             foreach (PlanEntity item in this.PlanList)
//             {
//                 this.FilterPlanList.Add(item);
//             }
            for (int pos = 0; pos < PlanList.Count; pos ++)
            {
                this.FilterPlanList.Add(PlanList[pos]);
            }

            OnAddPlan = new DelegateCommand(OnAddPlanCommand, CanAdd);
            OnEditPlan = new DelegateCommand(OnEditPlanCommand, CanEdit);
            OnDeletePlan = new DelegateCommand(OnDeletePlanCommand, CanEdit);
            OnInsertPlan = new DelegateCommand(OnInsertPlanCommand, CanEdit);

            OnOK = new DelegateCommand(OnOKCommand, CanOKCancel);
            OnCancel = new DelegateCommand(OnCancelCommand, CanOKCancel);

            OnSelectionChanged = new DelegateCommand(OnSelectionChangedCommand);
            DoubleClickProject = new DelegateCommand(DoubleClickProjectCommand);
        }

        public ICommand OnAddPlan { get; private set; }

        public ICommand OnEditPlan { get; private set; }

        public ICommand OnDeletePlan { get; private set; }

        public ICommand OnInsertPlan { get; private set; }

        public ICommand OnOK { get; private set; }

        public ICommand OnCancel { get; private set; }

        public ICommand OnSelectionChanged { get; private set; }

        public ICommand DoubleClickProject { get; private set; }

        private void DoubleClickProjectCommand()
        {
            OnEditPlanCommand();
        }

        public void OnFilterCommand(string projectName, string manufactureNumber, bool showFinished, bool showUnfinished,
            Nullable<DateTime> startTargetDate, Nullable<DateTime> endTargetDate, 
            Nullable<DateTime> startAccomplishDate, Nullable<DateTime> endAccomplishDate,
            bool showOvertime)
        {
            FilterPlanList.Clear();
            for (int pos = 0; pos < PlanList.Count; pos++)
            {
                PlanEntity item = PlanList[pos];
                DateTime targetDateTime = item.TargetDateAdjustment2.HasValue ? item.TargetDateAdjustment2.Value :
                                        (item.TargetDateAdjustment1.HasValue ? item.TargetDateAdjustment1.Value : item.TargetDate);
                if (null != projectName && string.Empty != projectName && projectName != item.ProjectName)
                {
                    continue;
                }

                if (null != manufactureNumber && string.Empty != manufactureNumber && manufactureNumber != item.ManufactureNumber)
                {
                    continue;
                }

                if (null != startTargetDate && targetDateTime < startTargetDate)
                {
                    continue;
                }

                if (null != endTargetDate && targetDateTime > endTargetDate)
                {
                    continue;
                }

                if (!showFinished && item.AccomplishDate.HasValue)
                {
                    continue;
                }

                if (!showUnfinished && !item.AccomplishDate.HasValue)
                {
                    continue;
                }

                if (null != startAccomplishDate 
                    && (!item.AccomplishDate.HasValue || item.AccomplishDate.Value < startAccomplishDate))
                {
                    continue;
                }

                if (null != endAccomplishDate
                    && (!item.AccomplishDate.HasValue || item.AccomplishDate.Value > endAccomplishDate))
                {
                    continue;
                }

                if (showOvertime)
                {
                    DateTime today = DateTime.Now;
                    if (item.AccomplishDate.HasValue)
                    {
                        if (item.AccomplishDate.Value <= today)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (targetDateTime >= today)
                        {
                            continue;
                        }
                    }
                }

//                 if (totalValueDictionary.ContainsKey(item.SheetName))
//                 {
//                     totalValueDictionary[item.SheetName] += item.Weight;
//                     if (item.AccomplishDate.HasValue && item.Score.HasValue)
//                     {
//                         accomplishValueDictionary[item.SheetName] += item.Score.Value;
//                     }
//                 }
//                 else
//                 {
//                     totalValueDictionary.Add(item.SheetName, item.Weight);
//                     accomplishValueDictionary.Add(item.SheetName, item.Score.HasValue ? item.Score.Value : 0);
//                 }
                FilterPlanList.Add(item);
            }

//             Dictionary<string, int> currentAccomplishRateDictionary = new Dictionary<string, int>();
//             foreach (KeyValuePair<string, decimal> kv in totalValueDictionary)
//             {
//                 decimal resultValue = (0 == kv.Value) ? 0 : accomplishValueDictionary[kv.Key] / kv.Value;
//                 int resultInt = Convert.ToInt16(Convert.ToDouble(resultValue) * 100);
//                 currentAccomplishRateDictionary.Add(kv.Key, resultInt);
//             }
//             AccomplishRateDictionary = currentAccomplishRateDictionary;
        }

        public void OnFilterFinishedCommand(bool aShowFinished, bool aShowUnfinished)
        {
            FilterPlanList.Clear();
            for (int pos = 0; pos < PlanList.Count; pos++)
            {
                PlanEntity item = PlanList[pos];          
                if((aShowFinished && item.AccomplishDate.HasValue)
                    || (aShowUnfinished && !item.AccomplishDate.HasValue))
                {
                    FilterPlanList.Add(item);
                }
            }
        }

        public void OnFilterFinishedCommand(bool aDateFilter, Nullable<DateTime> aStart, Nullable<DateTime> aEnd)
        {
            FilterPlanList.Clear();
            for (int pos = 0; pos < PlanList.Count; pos++)
            {
                PlanEntity item = PlanList[pos];
                DateTime targetDateTime = item.TargetDateAdjustment2.HasValue ? item.TargetDateAdjustment2.Value :
                                        (item.TargetDateAdjustment1.HasValue ? item.TargetDateAdjustment1.Value : item.TargetDate);

                do
                {
                    if (aDateFilter)
                    {
                        if (aStart.HasValue && aEnd.HasValue)
                        {
                            if (!((targetDateTime >= aStart && targetDateTime <= aEnd)
                                || (item.AccomplishDate.HasValue && item.AccomplishDate.Value >= aStart && item.AccomplishDate.Value <= aEnd)))
                            {
                                break;
                            }
                        }
                        else if (aStart.HasValue)
                        {
                            if (!(targetDateTime >= aStart || (item.AccomplishDate.HasValue && item.AccomplishDate.Value >= aStart)))
                            {
                                break;
                            }
                        }
                        else if (aEnd.HasValue)
                        {
                            if (!(targetDateTime <= aEnd || (item.AccomplishDate.HasValue && item.AccomplishDate.Value <= aEnd)))
                            {
                                break;
                            }
                        }

                    }
                    FilterPlanList.Add(item);                    
                } while (false);
            }
        }

        private void OnAddPlanCommand()
        {
            IsEditing = true;
            PlanEntity newPlanEntity = new PlanEntity();
            int newSequenceId = 1;
            if (PlanList.Count > 0)
            {
                newPlanEntity.ProjectName = PlanList[0].ProjectName;
                newPlanEntity.SheetName = PlanList[0].SheetName;
                newPlanEntity.VersionId = PlanList[0].VersionId;
                newPlanEntity.ManufactureNumber = PlanList[0].ManufactureNumber;
                newPlanEntity.TargetDate = DateTime.Now;
                newPlanEntity.Weight = Convert.ToDecimal(0);
                newSequenceId = GetNewSequenceId(newSequenceId);
                
            }
            newPlanEntity.SequenceId = newSequenceId;
            EditingPlanEntity = newPlanEntity;
            uneditedPlanEntity = null;
            SelectPlanEntity = null;
            UpdateChanged("EditingOriginalName");
        }

        private int GetNewSequenceId(int newSequenceId)
        {
            bool isUsed = false;
            do
            {
                isUsed = false;
                foreach (PlanEntity item in PlanList)
                {
                    if (newSequenceId == item.SequenceId)
                    {
                        isUsed = true;
                        break;
                    }
                }
                if (isUsed)
                {
                    ++newSequenceId;
                }
            } while (isUsed); 
            return newSequenceId;
        }

        private void CopyPlanEntity(PlanEntity aFrom, ref PlanEntity aTo)
        {
            aTo.ProjectName = aFrom.ProjectName;
            aTo.SequenceId = aFrom.SequenceId;
            aTo.AccomplishDate = aFrom.AccomplishDate;
            aTo.ComponentName = aFrom.ComponentName;
            aTo.DepartmentId = aFrom.DepartmentId;
            aTo.DepartmentName = aFrom.DepartmentName;
            aTo.ManufactureNumber = aFrom.ManufactureNumber;
            aTo.Remark = aFrom.Remark;
            aTo.Score = aFrom.Score;
            aTo.SheetName = aFrom.SheetName;
            aTo.TargetDate = aFrom.TargetDate;
            aTo.TargetDateAdjustment1 = aFrom.TargetDateAdjustment1;
            aTo.TargetDateAdjustment2 = aFrom.TargetDateAdjustment2;
            aTo.TaskDescription = aFrom.TaskDescription;
            aTo.VersionId = aFrom.VersionId;
            aTo.Weight = aFrom.Weight;
            //aTo.DUpdate();
        }

        private void OnEditPlanCommand()
        {
            if (IsEditing)
            {
                if (null != uneditedPlanEntity && uneditedPlanEntity.SequenceId == SelectPlanEntity.SequenceId)
                {
                    return;
                }

                CustomMessage customMesage = new CustomMessage("保存编辑并切换?", CustomMessage.MessageType.Confirm);
                customMesage.Closed += new EventHandler(EditingConfirm_Closed);
                customMesage.Show();
            }
            else
            {
                IsEditing = true;
                uneditedPlanEntity = new PlanEntity();
                CopyPlanEntity(SelectPlanEntity, ref uneditedPlanEntity);
                PlanEntity planEntity = new PlanEntity();
                CopyPlanEntity(SelectPlanEntity, ref planEntity);
                EditingPlanEntity = planEntity;
                UpdateChanged("EditingOriginalName");
            }
        }

        private void OnDeletePlanCommand()
        {
            if (SelectPlanEntity != null)
            {
                CustomMessage customMesage = new CustomMessage("删除选中项?", CustomMessage.MessageType.Confirm);
                customMesage.Closed += new EventHandler(DeleteConfirm_Closed);
                customMesage.Show();
            }
        }

        private void OnInsertPlanCommand()
        {
            if (SelectPlanEntity != null)
            {
                int lNewSequenceId = SelectPlanEntity.SequenceId + 1;
                bool autoIncrease = false;
                for (int pos = 0; pos < PlanList.Count; ++pos)
                {
                    PlanEntity item = PlanList[pos];
                    if (item.SequenceId == SelectPlanEntity.SequenceId)
                    {
                        autoIncrease = true;
                    }

                    if (autoIncrease && item.SequenceId > SelectPlanEntity.SequenceId)
                    {
                        ++item.SequenceId;
                    }
                }

                IsEditing = true;
                PlanEntity newPlanEntity = new PlanEntity();
                newPlanEntity.ProjectName = SelectPlanEntity.ProjectName;
                newPlanEntity.SheetName = SelectPlanEntity.SheetName;
                newPlanEntity.VersionId = SelectPlanEntity.VersionId;
                newPlanEntity.ManufactureNumber = PlanList[0].ManufactureNumber;
                newPlanEntity.TargetDate = DateTime.Now;
                newPlanEntity.Weight = Convert.ToDecimal(0);
                newPlanEntity.SequenceId = SelectPlanEntity.SequenceId + 1;
                EditingPlanEntity = newPlanEntity;
                uneditedPlanEntity = null;
                SelectPlanEntity = null;
                UpdateChanged("EditingOriginalName");
            }
        }

        public void OnOKCommand()
        {
            if (!editingPlanEntity.AccomplishDate.HasValue && editingPlanEntity.Score.HasValue)
            {
                Message.ErrorMessage("未设置完成时间！");
                return;
            }
            else if (editingPlanEntity.AccomplishDate.HasValue && !editingPlanEntity.Score.HasValue)
            {
                Message.ErrorMessage("未设置分数！");
                return;
            }

            if (editingPlanEntity.Score.HasValue && editingPlanEntity.Score.Value.CompareTo(editingPlanEntity.Weight) > 0)
            {
                Message.ErrorMessage("实际得分大于权重分值！");
                return;
            }

            do
            {
                if (null != uneditedPlanEntity && uneditedPlanEntity.SequenceId == editingPlanEntity.SequenceId)
                {
                    break;
                }

                foreach (PlanEntity item in PlanList)
                {
                    if (null != uneditedPlanEntity && uneditedPlanEntity == item)
                    {
                        continue;
                    }

                    if (editingPlanEntity.SequenceId == item.SequenceId)
                    {
                        editingPlanEntity.SequenceId = (null != uneditedPlanEntity && null != uneditedPlanEntity.SequenceId) ? uneditedPlanEntity.SequenceId : GetNewSequenceId(editingPlanEntity.SequenceId);
                        Message.ErrorMessage("序列号重复！");
                        return;
                    }
                }
            } while (false);

            string getDepartmentName = string.Empty;
            if (DepartmentIdNameDictionary.TryGetValue(editingPlanEntity.DepartmentId, out getDepartmentName))
            {
                editingPlanEntity.DepartmentName = getDepartmentName;
            }
            editingPlanEntity.SheetName = this.Title;
            if (PlanList.Count > 0)
            {
                editingPlanEntity.ManufactureNumber = PlanList[0].ManufactureNumber;
            }
            //editingPlanEntity.DUpdate();
            if (null == uneditedPlanEntity)
            {
                PlanList.Add(editingPlanEntity);
            }
            else
            {
                for (int pos = 0; pos < PlanList.Count; ++pos)
                {
                    PlanEntity item = PlanList[pos];
                    if (item.ManufactureNumber == uneditedPlanEntity.ManufactureNumber
                        && item.VersionId == uneditedPlanEntity.VersionId
                        && item.SequenceId == uneditedPlanEntity.SequenceId)
                    {
                        CopyPlanEntity(editingPlanEntity, ref item);
                        break;
                    }
                }
            }

            IsEditing = false;
            EditingPlanEntity = new PlanEntity();
            uneditedPlanEntity = null;
            OnSelectionChangedCommand();
            UpdateChanged("EditingOriginalName");
        }

        private void OnCancelCommand()
        {
            IsEditing = false;
            EditingPlanEntity = new PlanEntity();
            uneditedPlanEntity = null;
            OnSelectionChangedCommand();
            UpdateChanged("EditingOriginalName");
        }

        private void OnSelectionChangedCommand()
        {
            if (!isEditing)
            {
                if (null != SelectPlanEntity)
                {
                    PlanEntity planEntity = new PlanEntity();
                    CopyPlanEntity(SelectPlanEntity, ref planEntity);
                    EditingPlanEntity = planEntity;
                }
                else
                {
                    EditingPlanEntity = new PlanEntity();
                }
            }
            else
            {
                if (null == SelectPlanEntity && null == uneditedPlanEntity)
                {
                    return;
                }

                if (null != SelectPlanEntity && null != uneditedPlanEntity
                    && SelectPlanEntity.ManufactureNumber == uneditedPlanEntity.ManufactureNumber
                    && SelectPlanEntity.VersionId == uneditedPlanEntity.VersionId
                    && SelectPlanEntity.SequenceId == uneditedPlanEntity.SequenceId
                    && SelectPlanEntity.SheetName == uneditedPlanEntity.SheetName)
                {
                    return;
                }
                CustomMessage customMesage = new CustomMessage("保存编辑并切换?", CustomMessage.MessageType.Confirm);
                customMesage.Closed += new EventHandler(SelectionConfirm_Closed);
                customMesage.Show();
            }
        }

        private void EditingConfirm_Closed(object sender, EventArgs e)
        {
            CustomMessage customMesage = sender as CustomMessage;
            if (customMesage.DialogResult == true)
            {
                OnOKCommand();
                OnEditPlanCommand();
            }
        }

        private void SelectionConfirm_Closed(object sender, EventArgs e)
        {
            CustomMessage customMesage = sender as CustomMessage;
            if (customMesage.DialogResult == true)
            {
                OnOKCommand();
            }
            else
            {
                if (null == uneditedPlanEntity)
                {
                    SelectPlanEntity = null;
                }
                else
                {
                    foreach (PlanEntity item in PlanList)
                    {
                        if (item.ManufactureNumber == uneditedPlanEntity.ManufactureNumber
                            && item.VersionId == uneditedPlanEntity.VersionId
                            && item.SequenceId == uneditedPlanEntity.SequenceId
                            && item.SheetName == uneditedPlanEntity.SheetName)
                        {
                            SelectPlanEntity = item;
                            break;
                        }
                    }
                }
            }
        }

        private void DeleteConfirm_Closed(object sender, EventArgs e)
        {
            CustomMessage customMesage = sender as CustomMessage;
            if (customMesage.DialogResult == true)
            {
                IsEditing = false;
                if (PlanList.Contains(SelectPlanEntity))
                {
                    DeletedPlanList.Add(SelectPlanEntity);
                    PlanList.Remove(SelectPlanEntity);
                }
                SelectPlanEntity = null;
                EditingPlanEntity = new PlanEntity();
                uneditedPlanEntity = null;
                UpdateChanged("EditingOriginalName");
                IsChanged = true;
            }
        }
    }
}
