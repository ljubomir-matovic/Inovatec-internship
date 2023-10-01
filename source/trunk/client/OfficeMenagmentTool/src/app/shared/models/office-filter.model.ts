export interface OfficeFilter{
  Name?: string,
  PageNumber: number,
  PageSize?: number,
  SortField?: string | null,
  SortOrder: -1 | 1;
}
