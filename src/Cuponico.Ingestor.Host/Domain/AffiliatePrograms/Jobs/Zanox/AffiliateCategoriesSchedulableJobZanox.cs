﻿using Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Categories;

namespace Cuponico.Ingestor.Host.Domain.AffiliatePrograms.Jobs.Zanox
{
    public class AffiliateCategoriesSchedulableJobZanox: AffiliateCategoriesSchedulableJob
    {
        public AffiliateCategoriesSchedulableJobZanox(IAffiliateCategoryRepository repositoryFromPartner, IAffiliateCategoryRepository cuponicoRepository) : base(repositoryFromPartner, cuponicoRepository)
        {
        }
    }
}
