using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeLabelPrinting.Interfaces
{
	public interface IFile : IDisposable
	{
		byte[] FileBytes { get; }
		string FileName { get; set; }
	}
}
