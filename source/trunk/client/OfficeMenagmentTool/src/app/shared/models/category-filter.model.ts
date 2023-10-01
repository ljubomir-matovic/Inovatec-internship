import { CategoryTypes } from "../constants/category-types";

export interface CategoryFilter {
  Name?: string | null,
  Types?: CategoryTypes[] | null,
  PageNumber: number,
  PageSize: number,
  SortField?: string | null,
  SortOrder: -1 | 1
}
