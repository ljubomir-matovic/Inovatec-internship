export interface DataPage<T> {
  totalRecords: number;
  more: boolean;
  data: Array<T>;
}
