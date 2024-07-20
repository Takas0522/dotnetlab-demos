export abstract class MyDayaBase {
  abstract code: number;
  abstract name: string;
}

export interface MyData extends MyDayaBase {
  code: number;
  name: string;
}

export interface NewMyData extends MyDayaBase {
  code: number;
  name: string;
  newData: string;
}