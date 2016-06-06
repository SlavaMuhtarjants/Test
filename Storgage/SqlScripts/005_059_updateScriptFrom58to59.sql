create
 index idx_PhotoLibrary_SpaceId
    on dbo.PhotoLibrary (SpaceId) 
        include (Id, Link)
go


create 
 unique
 index idx_FilterRootDictionary_FilterId
    on [dbo].[FilterRootDictionary] ([FilterId], RootDictionaryId)
       include (Id)

go