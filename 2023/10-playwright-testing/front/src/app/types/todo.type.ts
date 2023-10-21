export type ToDo = {
  id: number;
  description: string;
  status: 'completed' | 'incompleted';
  addDate: Date;
  updateDate: Date;
};