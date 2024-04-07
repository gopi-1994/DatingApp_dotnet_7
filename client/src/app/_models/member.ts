import { Photo } from "./photo"


export interface Member {
    id: number
    userName: string
    photoUrl: string
    age: number
    // passwordHash: string
    // passwordSalt: string
    dateOfBirth: string
    created: string
    lastActive: string
    knownAs: string
    gender: string
    introduction: any
    lookingFor: string
    interests: string
    city: string
    country: string
    photos: Photo[]
  }
  
