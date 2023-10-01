import { Office } from "./office";

export interface User {
  id: number,
  firstName: string,
  lastName: string,
  email: string,
  role: number,
  dateCreated: string,
  officeId: number
}
