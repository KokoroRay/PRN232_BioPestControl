export interface Chemical {
  id: number;
  name: string;
  vietnameseName?: string;
  casNumber?: string;
  chemicalGroup?: string;
  chemicalFormula?: string;
  description?: string;
  toxicityLevel?: string;
  usageMethod?: string;
  safetyNotes?: string;
  targetCrops?: string;
  targetPests?: string;
  isActive: boolean;
}

export interface CreateChemicalRequest {
  name: string;
  vietnameseName?: string;
  casNumber?: string;
  chemicalGroup?: string;
  chemicalFormula?: string;
  description?: string;
  toxicityLevel?: string;
  usageMethod?: string;
  safetyNotes?: string;
  targetCrops?: string;
  targetPests?: string;
  isActive?: boolean;
}
