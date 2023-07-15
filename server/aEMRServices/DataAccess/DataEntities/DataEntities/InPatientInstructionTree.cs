using eHCMS.Services.Core.Base;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataEntities
{
    public class InPatientInstructionTree : NotifyChangedBase
    {
        public InPatientInstructionTree(RefDepartment aDepartment)
        {
            Department = aDepartment;
            NodeText = Department.DeptName;
        }
        public InPatientInstructionTree(InPatientInstruction aInPatientInstruction)
        {
            Instruction = aInPatientInstruction;
            NodeText = Instruction.InstructionDate.ToString("HH:mm dd/MM/yyy");
        }
        private string _NodeText;
        [DataMember]
        public string NodeText
        {
            get
            {
                return _NodeText;
            }
            set
            {
                _NodeText = value;
                RaisePropertyChanged("NodeText");
            }
        }
        private List<InPatientInstructionTree> _Children;
        [DataMember]
        public List<InPatientInstructionTree> Children
        {
            get
            {
                return _Children;
            }
            set
            {
                _Children = value;
                RaisePropertyChanged("Children");
            }
        }
        private RefDepartment _Department;
        [DataMember]
        public RefDepartment Department
        {
            get
            {
                return _Department;
            }
            set
            {
                _Department = value;
                RaisePropertyChanged("Department");
            }
        }
        private InPatientInstruction _Instruction;
        [DataMember]
        public InPatientInstruction Instruction
        {
            get
            {
                return _Instruction;
            }
            set
            {
                _Instruction = value;
                RaisePropertyChanged("Instruction");
            }
        }
    }
}
