export interface ItemFilter {
  Name?: string | null,
  CategoryType?: number | null,
  Categories?: number[] | null,
  PageNumber: number,
  PageSize: number,
  SortField?: string | null,
  SortOrder: -1 | 1
}
