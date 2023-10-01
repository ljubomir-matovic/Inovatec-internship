import { Role } from "../constants/role";

export interface UserFilter{
  PageNumber:number,
  PageSize?: number,
  SortField?:string | null,
  SortOrder:-1|1;
  FirstName?:string | null,
  LastName?:string | null,
  Email?:string | null,
  Office?: string | null,
  Roles?:Role[] | null,
  Offices?: number[] | null
}
